module Pa

open System
open System.IO
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open FSharp.NativeInterop

[<Struct>]
type pa_context_flags =
    | PA_CONTEXT_NOFLAGS
    | PA_CONTEXT_NOAUTOSPAWN
    | PA_CONTEXT_NOFAIL

[<Struct>]
type pa_context_state_t =
    | PA_CONTEXT_UNCONNECTED
    | PA_CONTEXT_CONNECTING
    | PA_CONTEXT_AUTHORIZING
    | PA_CONTEXT_SETTING_NAME
    | PA_CONTEXT_READY
    | PA_CONTEXT_FAILED
    | PA_CONTEXT_TERMINATED

[<Struct>]
type pa_sample_format_t =
    | PA_SAMPLE_MAX
    | PA_SAMPLE_INVALID
    | PA_SAMPLE_U8
    | PA_SAMPLE_ALAW
    | PA_SAMPLE_ULAW
    | PA_SAMPLE_S16LE
    | PA_SAMPLE_S16BE
    | PA_SAMPLE_FLOAT32LE
    | PA_SAMPLE_FLOAT32BE
    | PA_SAMPLE_S32LE
    | PA_SAMPLE_S32BE
    | PA_SAMPLE_S24LE
    | PA_SAMPLE_S24BE
    | PA_SAMPLE_S24_32LE
    | PA_SAMPLE_S24_32BE

[<Struct>]
type pa_channel_position_t =
    | PA_CHANNEL_POSITION_INVALID

[<StructLayout(LayoutKind.Sequential)>]
type pa_context = struct end

[<StructLayout(LayoutKind.Sequential)>]
type pa_mainloop = struct end

[<StructLayout(LayoutKind.Sequential)>]
type pa_mainloop_api = struct end

[<StructLayout(LayoutKind.Sequential)>]
type pa_operation = struct end

[<StructLayout(LayoutKind.Sequential)>]
type pa_channel_map = struct
    val mutable channels: uint8
    val mutable map: pa_channel_position_t
end

[<StructLayout(LayoutKind.Sequential)>]
type pa_sample_spec = struct
    val mutable format:pa_sample_format_t
    val mutable rate: uint32
    val mutable channels: uint8
end

[<StructLayout(LayoutKind.Sequential)>]
type pa_server_info = struct
    val mutable user_name: string 
    val mutable host_name: string
    val mutable server_version: string
    val mutable server_name: string
    val mutable sample_spec: pa_sample_spec
    val mutable default_sink_name: string
    val mutable default_source_name: string
    val mutable cookie: uint32
    val mutable channel_map: pa_channel_map
end

type pa_volume_t = uint32

[<StructLayout(LayoutKind.Sequential)>]
type pa_cvolume = struct 
    val mutable channels: uint8
    [<MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)>]
    val mutable values: pa_volume_t[]
end

type pa_usec_t = uint64

[<Struct>]
type pa_sink_flags_t = 
    | PA_SINK_NOFLAGS = 0x0000u
    | PA_SINK_HW_VOLUME_CTRL = 0x0001u
    | PA_SINK_LATENCY = 0x0002u
    | PA_SINK_HARDWARE = 0x0004u
    | PA_SINK_NETWORK = 0x0008u
    | PA_SINK_HW_MUTE_CTRL = 0x0010u
    | PA_SINK_DECIBEL_VOLUME = 0x0020u
    | PA_SINK_FLAT_VOLUME = 0x0040u
    | PA_SINK_DYNAMIC_LATENCY = 0x0080u
    | PA_SINK_SET_FORMATS = 0x0100u
    | PA_SINK_SHARE_VOLUME_WITH_MASTER = 0x1000000u
    | PA_SINK_DEFERRED_VOLUME = 0x2000000u

[<Struct>]
type pa_sink_state_t = 
    | PA_SINK_INVALID_STATE = -1
    | PA_SINK_RUNNING = 0
    | PA_SINK_IDLE = 1
    | PA_SINK_SUSPENDED = 2
    | PA_SINK_UNLINKED = -3

[<StructLayout(LayoutKind.Sequential)>]
type pa_sink_port_info = struct end

[<StructLayout(LayoutKind.Sequential)>]
type pa_format_info = struct end

[<StructLayout(LayoutKind.Sequential)>]
type pa_proplist = struct end

[<StructLayout(LayoutKind.Sequential)>]
type pa_sink_info = struct
    val mutable name: string
    val mutable index: uint32
    val mutable description: string
    val mutable sample_spec: pa_sample_spec 
    val mutable channel_map: pa_channel_map 
    val mutable owner_module: uint32 
    val mutable volume: pa_cvolume 
    val mutable mute: int 
    val mutable monitor_source: uint32 
    val mutable monitor_source_name: string
    val mutable latency: pa_usec_t 
    val mutable driver: string
    val mutable flags: pa_sink_flags_t 
    val mutable proplist: pa_proplist nativeptr
    val mutable configured_latency: pa_usec_t 
    val mutable base_volume: pa_volume_t 
    val mutable state: pa_sink_state_t 
    val mutable n_volume_steps: uint32 
    val mutable card: uint32 
    val mutable n_ports: uint32 
    val mutable ports: IntPtr //pa_sink_port_info nativeptr
    val mutable active_port: pa_sink_port_info nativeptr
    val mutable n_formats: uint8 
    val mutable formats: IntPtr //pa_format_info nativeptr
end

type void_cb_t = delegate of unit -> unit

[<StructLayout(LayoutKind.Sequential)>]
type pa_spawn_api = struct
    val mutable prefork: void_cb_t
    val mutable postfork: void_cb_t
    val mutable atfork: void_cb_t
    new(x, y, z) = { prefork = x; postfork = y; atfork = z }
end

type pa_context_notify_cb_t =
    delegate of nativeptr<pa_context> * nativeint -> unit

type pa_sink_info_cb_t =
    delegate of nativeptr<pa_context> * IntPtr * int * nativeint -> unit

type pa_server_info_cb_t =
    delegate of nativeptr<pa_context> * pa_server_info byref * nativeint -> unit

[<DllImport ("libpulse", EntryPoint="pa_mainloop_new",CallingConvention=CallingConvention.Cdecl)>]
extern pa_mainloop* pa_mainloop_new ()

[<DllImport ("libpulse", EntryPoint="pa_mainloop_get_api",CallingConvention=CallingConvention.Cdecl)>]
extern pa_mainloop_api* pa_mainloop_get_api (pa_mainloop *a)

[<DllImport ("libpulse", EntryPoint="pa_context_new",CallingConvention=CallingConvention.Cdecl)>]
extern pa_context* pa_context_new (pa_mainloop_api *a, string name)

[<DllImport ("libpulse", EntryPoint="pa_context_connect",CallingConvention=CallingConvention.Cdecl)>]
extern int pa_context_connect (
    pa_context *c,
    string server,
    pa_context_flags flags,
    pa_spawn_api& api)

[<DllImport ("libpulse", EntryPoint="pa_context_set_state_callback",CallingConvention=CallingConvention.Cdecl)>]
extern void pa_context_set_state_callback (
    pa_context *c,
    pa_context_notify_cb_t cb,
    void *userdata)

[<DllImport ("libpulse", EntryPoint="pa_context_get_state",CallingConvention=CallingConvention.Cdecl)>]
extern pa_context_state_t pa_context_get_state (
    pa_context *c)

[<DllImport ("libpulse", EntryPoint="pa_mainloop_iterate",CallingConvention=CallingConvention.Cdecl)>]
extern int pa_mainloop_iterate (
    pa_mainloop *c,
    int block,
    int& retval)

[<DllImport ("libpulse", EntryPoint="pa_mainloop_run",CallingConvention=CallingConvention.Cdecl)>]
extern int pa_mainloop_run (
    pa_mainloop *c,
    int& retval)

[<DllImport ("libpulse", EntryPoint="pa_context_get_server_info")>]
extern pa_operation* pa_context_get_server_info (
    pa_context *c,
    pa_server_info_cb_t cb,
    void *userdata)

[<DllImport ("libpulse", EntryPoint="pa_context_get_sink_info_by_name")>]
extern pa_operation* pa_context_get_sink_info_by_name (
    pa_context *c,
    string name,
    pa_sink_info_cb_t cb,
    void *userdata)

[<DllImport ("libpulse", EntryPoint="pa_context_get_sink_info_list",CallingConvention=CallingConvention.Cdecl)>]
extern pa_operation* pa_context_get_sink_info_list (
    pa_context *c,
    pa_sink_info_cb_t cb,
    void *userdata)

[<DllImport ("libpulse", EntryPoint="pa_cvolume_valid",CallingConvention=CallingConvention.Cdecl)>]
extern int pa_cvolume_valid (pa_cvolume& v)

[<DllImport ("libpulse", EntryPoint="pa_cvolume_avg",CallingConvention=CallingConvention.Cdecl)>]
extern pa_volume_t pa_cvolume_avg (pa_cvolume& v)

[<DllImport ("libpulse", EntryPoint="pa_cvolume_snprint",CallingConvention=CallingConvention.Cdecl)>]
extern string pa_cvolume_snprint (
    string s,
    int l,
    pa_cvolume& c)

[<DllImport ("libpulse", EntryPoint="pa_proplist_to_string_sep",CallingConvention=CallingConvention.Cdecl)>]
extern string pa_proplist_to_string_sep(IntPtr p, string sep);
