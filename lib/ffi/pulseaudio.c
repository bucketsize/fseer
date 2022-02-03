#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <assert.h>
#include <signal.h>
#include <pulse/pulseaudio.h>

#define CAML_NAME_SPACE
#include <caml/mlvalues.h>
#include <caml/memory.h>
#include <caml/alloc.h>
#include <caml/callback.h>
#include <caml/custom.h>

static void context_state_callback  (pa_context *c
        , void *userdata);
static void server_info_callback    (pa_context *c
        , const pa_server_info *i
        , void *userdata);
static void sink_info_callback      (pa_context *c
        , const pa_sink_info *i
        , int eol, void *userdata);
static void subscribe_callback      (pa_context *c
        , pa_subscription_event_type_t type
        , uint32_t idx
        , void *userdata);
static void exit_signal_callback    (pa_mainloop_api *m
        , pa_signal_event *e
        , int sig
        , void *userdata);
static int pa_connect();
static int quit(int);

static pa_mainloop* _mainloop = NULL;
static pa_mainloop_api* _mainloop_api = NULL;
static pa_context* _context = NULL;
static pa_signal_event* _signal = NULL;
static char* _userdata = "somedata";
static bool _context_connected = false;

static value connect_cb, sink_cb, volume_cb, muted_cb;
CAMLprim value caml_pa_connect_cb(value cb)
{
    CAMLparam1 (cb);
    connect_cb = cb;
    caml_register_global_root(&connect_cb);
    CAMLreturn (0);
}
CAMLprim value caml_pa_sink_cb(value cb)
{
    CAMLparam1 (cb);
    sink_cb = cb;
    caml_register_global_root(&sink_cb);
    CAMLreturn (0);
}
CAMLprim value caml_pa_volume_cb(value cb)
{
    CAMLparam1 (cb);
    volume_cb = cb;
    caml_register_global_root(&volume_cb);
    CAMLreturn (0);
}
CAMLprim value caml_pa_muted_cb(value cb)
{
    CAMLparam1 (cb);
    muted_cb = cb;
    caml_register_global_root(&muted_cb);
    CAMLreturn (0);
}
void exec_connect_cb() 
{
    if (connect_cb) 
    {
        caml_callback (connect_cb, 0);
    }
}
void exec_sink_cb(const char *sink) 
{
    if (sink_cb)
    {
        caml_callback (sink_cb, caml_copy_string(sink));
    }
}
void exec_volume_cb(float volume)
{
    if (volume_cb)
    {
        caml_callback (volume_cb, caml_copy_double(volume));
    }
}
void exec_muted_cb(bool muted)
{
    if (muted_cb)
    {
        caml_callback (muted_cb, Val_bool(muted));
    }
}

CAMLprim value caml_pa_tick(value unit)
{
    CAMLparam1 (unit);
    int ret = 1;

    // call a few time to move things faster
    for(int i=0; i<96; ++i)
    {
        if (pa_mainloop_iterate (_mainloop, 0, &ret) < 0)
        {
            fprintf (stderr, "cc> pa_mainloop_iterate() failed.\n");
            CAMLreturn (ret);
        }
    }
    CAMLreturn (ret);
}
CAMLprim value caml_pa_connect(value unit)
{
    CAMLparam1 (unit);
    int status = pa_connect ();
    if (status != 0)
    {
        CAMLreturn (1);
    }
    CAMLreturn (0);
}

static int quit(int ret)
{
    _mainloop_api->quit(_mainloop_api, ret);
    return ret;
}
static int pa_connect() {
    fprintf(stderr, "cc> pa connect\n");
    _mainloop = pa_mainloop_new();
    if (!_mainloop)
    {
        fprintf(stderr, "cc> pa_mainloop_new() failed.\n");
        return 1;
    }

    _mainloop_api = pa_mainloop_get_api(_mainloop);

    if (pa_signal_init(_mainloop_api) != 0)
    {
        fprintf(stderr, "cc> pa_signal_init() failed\n");
        return 1;
    }

    _signal = pa_signal_new(SIGINT, exit_signal_callback, (void*) &_userdata);
    if (!_signal)
    {
        fprintf(stderr, "cc> pa_signal_new() failed\n");
        return 1;
    }
    signal(SIGPIPE, SIG_IGN);

    _context = pa_context_new(_mainloop_api, "PulseAudio Test");
    if (!_context)
    {
        fprintf(stderr, "cc> pa_context_new() failed\n");
        return 1;
    }

    if (pa_context_connect(_context, NULL, PA_CONTEXT_NOAUTOSPAWN, NULL) < 0)
    {
        fprintf(stderr, "cc> pa_context_connect() failed: %s\n", pa_strerror(pa_context_errno(_context)));
        return 1;
    }

    pa_context_set_state_callback(_context, context_state_callback, (void*) &_userdata);
    return 0;
}

/*
 * Called on SIGINT.
 */
static void exit_signal_callback(pa_mainloop_api *m, pa_signal_event *e, int sig, void *userdata)
{
    quit(0);
}

/*
 * Called whenever the context status changes.
 */
static void context_state_callback(pa_context *c, void *userdata)
{
    assert(c && userdata);

    switch (pa_context_get_state(c))
    {
        case PA_CONTEXT_CONNECTING:
        case PA_CONTEXT_AUTHORIZING:
        case PA_CONTEXT_SETTING_NAME:
            break;

        case PA_CONTEXT_READY:
            fprintf(stderr, "cc> pa context ready\n");
            pa_context_get_server_info(c, server_info_callback, userdata);

            // Subscribe to sink events from the server. This is how we get
            // volume change notifications from the server.
            pa_context_set_subscribe_callback(c, subscribe_callback, userdata);
            pa_context_subscribe(c, PA_SUBSCRIPTION_MASK_SINK, NULL, NULL);
            _context_connected = true;
            exec_connect_cb();
            break;

        case PA_CONTEXT_TERMINATED:
            fprintf(stderr, "cc> pa context terminated\n");
            quit(0);
            break;

        case PA_CONTEXT_FAILED:
        default:
            fprintf(stderr, "cc> pa context failed: %s\n", pa_strerror(pa_context_errno(c)));
            quit(1);
            break;
    }
}

/*
 * Called when an event we subscribed to occurs.
 */
static void subscribe_callback(pa_context *c,
        pa_subscription_event_type_t type, uint32_t idx, void *userdata)
{
    unsigned facility = type & PA_SUBSCRIPTION_EVENT_FACILITY_MASK;
    //type &= PA_SUBSCRIPTION_EVENT_TYPE_MASK;

    pa_operation *op = NULL;

    switch (facility)
    {
        case PA_SUBSCRIPTION_EVENT_SINK:
            op = pa_context_get_sink_info_by_index(c, idx, sink_info_callback, userdata);
            break;

        default:
            assert(0); // Got event we aren't expecting.
            break;
    }

    if (op)
        pa_operation_unref(op);
}

/*
 * Called when the requested sink information is ready.
 */
static void sink_info_callback(pa_context *c, const pa_sink_info *i,
        int eol, void *userdata)
{
    if (i)
    {
        float volume = (float)pa_cvolume_avg(&(i->volume)) / (float)PA_VOLUME_NORM;
        exec_volume_cb(volume);
        exec_muted_cb(i->mute);
        fprintf(stderr, "cc> pa volume = %.0f%%%s\n", volume * 100.0f, i->mute ? " (muted)" : "");
    }
}

/*
 * Called when the requested information on the server is ready. This is
 * used to find the default PulseAudio sink.
 */
static void server_info_callback(pa_context *c, const pa_server_info *i,
        void *userdata)
{
    fprintf(stderr, "cc> default sink name = %s\n", i->default_sink_name);
    exec_sink_cb(i->default_sink_name);
    pa_context_get_sink_info_by_name(c, i->default_sink_name, sink_info_callback, userdata);
}
