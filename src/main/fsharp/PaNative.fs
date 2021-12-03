module Pa 

open System
open System.IO
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open FSharp.NativeInterop

[<Struct>]
type pa_mainloop = struct end

[<Struct>]
type pa_mainloop_api = struct end

[<Struct>]
type pa_context = struct end

[<Struct>]
type pa_userdata = struct end

[<Struct>]
type pa_server_info_cb_t = struct end

[<Struct>]
type pa_context_flags = 
    | PA_CONTEXT_NOFLAGS
    | PA_CONTEXT_NOAUTOSPAWN
    | PA_CONTEXT_NOFAIL

[<Struct>]
[<StructLayout(LayoutKind.Sequential)>]
type pa_spawn_api = 
    val prefork: IntPtr
    val postfork: IntPtr 
    val atfork: IntPtr

[<Struct>]
type pa_proplist = struct end

[<Struct>]
[<StructLayout(LayoutKind.Sequential)>]
type pa_operation = struct
    val mutable index: uint32
    val mutable name: IntPtr 
    val mutable argument: IntPtr
    val mutable n_used: uint32
    val mutable proplist: IntPtr
end

[<DllImport ("libpulse", EntryPoint="pa_mainloop_new",CallingConvention=CallingConvention.Cdecl)>]
extern pa_mainloop* pa_mainloop_new ()

[<DllImport ("libpulse", EntryPoint="pa_mainloop_get_api",CallingConvention=CallingConvention.Cdecl)>]
extern pa_mainloop_api* pa_mainloop_get_api (pa_mainloop& a)

[<DllImport ("libpulse", EntryPoint="pa_context_new",CallingConvention=CallingConvention.Cdecl)>]
extern pa_context* pa_context_new (pa_mainloop_api& a)

[<DllImport ("libpulse", EntryPoint="pa_context_connect",CallingConvention=CallingConvention.Cdecl)>]
extern int pa_context_connect (
    pa_context& c,
    string server,
    pa_context_flags flags,
    pa_spawn_api& api)

[<DllImport ("libpulse", EntryPoint="pa_context_get_server_info",CallingConvention=CallingConvention.Cdecl)>]
extern pa_operation* pa_context_get_server_info (
    pa_context& c,
    pa_server_info_cb_t& cb,
    pa_userdata& userdata)

