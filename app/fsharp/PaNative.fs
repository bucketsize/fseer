module Pa

open System
open System.IO
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open FSharp.NativeInterop

let PA_CHANNELS_MAX = 32u

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
    | PA_SAMPLE_MAX
    | PA_SAMPLE_INVALID

[<Struct>]
type pa_channel_position_t = //TODO: check and fix enums and structures
    | PA_CHANNEL_POSITION_INVALID = -1
    | PA_CHANNEL_POSITION_MONO = 0

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
    [<MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)>]
    val mutable map: pa_channel_position_t[]
    override this.ToString() = 
        sprintf "pa_channel_map[channels=%d] " this.channels
end

[<StructLayout(LayoutKind.Sequential)>]
type pa_sample_spec = struct
    val mutable format: pa_sample_format_t
    val mutable rate: uint32
    val mutable channels: uint8
    override this.ToString() = 
        sprintf "pa_sample_spec[format=%s, rate=%d, channels=%d] " 
            (this.format.ToString())
            this.rate 
            this.channels
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
    override this.ToString() = 
        sprintf "pa_server_info[server_name=%s, sample_spec=%s, default_sink_name=%s, default_source_name=%s, channel_map=%A] "
            this.server_name
            (this.sample_spec.ToString())
            this.default_sink_name
            this.default_source_name
            this.channel_map

end

type pa_volume_t = uint32

[<StructLayout(LayoutKind.Sequential)>]
type pa_cvolume = struct 
    val mutable channels: uint8
    [<MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)>]
    val mutable values: pa_volume_t[]
    override this.ToString() = 
        sprintf "pa_cvolume=[channels=%d] " this.channels
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
    override this.ToString() = 
        sprintf "pa_sink_info[name=%s, index=%d, sample_spec=%s, channel_map=%s, owner_module=%d, volume=%s, monitor_source_name=%s, driver=%s, state=%s, card=%d] "
            this.name
            this.index
            (this.sample_spec.ToString())
            (this.channel_map.ToString())
            this.owner_module
            (this.volume.ToString())
            this.monitor_source_name
            this.driver
            (this.state.ToString())
            this.card
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
    delegate of nativeptr<pa_context> * IntPtr -> unit

type pa_sink_info_cb_t =
    delegate of nativeptr<pa_context> * IntPtr * int * IntPtr -> unit

type pa_server_info_cb_t =
    delegate of nativeptr<pa_context> * IntPtr * IntPtr -> unit

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

[<DllImport ("libpulse", EntryPoint="pa_context_set_state_callback",CallingConvention=CallingConvention.Cdecl)>]
extern void pa_context_set_state_callback (
    pa_context *c,
    pa_context_notify_cb_t cb,
    IntPtr userdata)

[<DllImport ("libpulse", EntryPoint="pa_context_get_server_info")>]
extern pa_operation* pa_context_get_server_info (
    pa_context *c,
    pa_server_info_cb_t cb,
    IntPtr userdata)

[<DllImport ("libpulse", EntryPoint="pa_context_get_sink_info_by_name")>]
extern pa_operation* pa_context_get_sink_info_by_name (
    pa_context *c,
    string name,
    pa_sink_info_cb_t cb,
    IntPtr userdata)

[<DllImport ("libpulse", EntryPoint="pa_context_get_sink_info_list",CallingConvention=CallingConvention.Cdecl)>]
extern pa_operation* pa_context_get_sink_info_list (
    pa_context *c,
    pa_sink_info_cb_t cb,
    IntPtr userdata)

[<DllImport ("libpulse", EntryPoint="pa_cvolume_valid",CallingConvention=CallingConvention.Cdecl)>]
extern int pa_cvolume_valid (pa_cvolume& v)

[<DllImport ("libpulse", EntryPoint="pa_cvolume_avg")>]
extern pa_volume_t pa_cvolume_avg (pa_cvolume& v)

[<DllImport ("libpulse", EntryPoint="pa_sw_volume_to_dB")>]
extern double pa_sw_volume_to_dB(pa_volume_t v)

[<DllImport ("libpulse", EntryPoint="pa_sw_volume_to_linear")>]
extern double pa_sw_volume_to_linear(pa_volume_t v)

[<DllImport ("libpulse", EntryPoint="pa_proplist_to_string")>]
extern string pa_proplist_to_string(pa_proplist *p);
