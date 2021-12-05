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

type Callback = delegate of unit -> unit

type pa_context_notify_cb_t = delegate of IntPtr * IntPtr -> unit 


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

[<Struct>]
[<StructLayout(LayoutKind.Sequential)>]
type pa_channel_map = struct
 val channels: uint8
 val map:pa_channel_position_t
end

[<Struct>]
[<StructLayout(LayoutKind.Sequential)>]
type pa_sample_spec = struct
    val format:pa_sample_format_t
    val rate: uint32
    val channels: uint8
end

[<Struct>]
[<StructLayout(LayoutKind.Sequential)>]
type pa_server_info = struct
    val user_name: string
    val host_name: string
    val server_version: string
    val server_name: string
    val sample_spec: pa_sample_spec
    val default_sink_name: string
    val default_source_name: string
    val cookie: uint32
    val channel_map: pa_channel_map
end

type pa_server_info_cb_t = 
    delegate of IntPtr * byref<pa_server_info> * IntPtr -> unit

[<Struct>]
[<StructLayout(LayoutKind.Sequential)>]
type pa_spawn_api = struct
        val prefork: Callback
        val postfork: Callback 
        val atfork: Callback
        new(x, y, z) = { prefork = x; postfork = y; atfork = z }
    end

[<Struct>]
type pa_proplist = struct end

[<DllImport ("libpulse", EntryPoint="pa_mainloop_new",CallingConvention=CallingConvention.Cdecl)>]
extern IntPtr pa_mainloop_new ()

[<DllImport ("libpulse", EntryPoint="pa_mainloop_get_api",CallingConvention=CallingConvention.Cdecl)>]
extern IntPtr pa_mainloop_get_api (IntPtr a)

[<DllImport ("libpulse", EntryPoint="pa_context_new",CallingConvention=CallingConvention.Cdecl)>]
extern IntPtr pa_context_new (IntPtr a, string name)

[<DllImport ("libpulse", EntryPoint="pa_context_connect",CallingConvention=CallingConvention.Cdecl)>]
extern Int32 pa_context_connect (
    IntPtr c,
    string server,
    pa_context_flags flags,
    pa_spawn_api& api)

[<DllImport ("libpulse", EntryPoint="pa_context_set_state_callback",CallingConvention=CallingConvention.Cdecl)>]
extern void pa_context_set_state_callback (
    IntPtr c,
    pa_context_notify_cb_t cb,
    IntPtr userdata)

[<DllImport ("libpulse", EntryPoint="pa_context_get_state",CallingConvention=CallingConvention.Cdecl)>]
extern pa_context_state_t pa_context_get_state (IntPtr c)

[<DllImport ("libpulse", EntryPoint="pa_context_get_server_info",CallingConvention=CallingConvention.Cdecl)>]
extern IntPtr pa_context_get_server_info (
    IntPtr c,
    pa_server_info_cb_t cb,
    IntPtr usedata)

[<DllImport ("libpulse", EntryPoint="pa_mainloop_iterate",CallingConvention=CallingConvention.Cdecl)>]
extern int pa_mainloop_iterate (
    IntPtr m,
    int block,
    int& retval)
