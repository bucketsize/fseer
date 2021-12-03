module Pa2 

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


type Callback = delegate of unit -> unit

type pa_context_notify_cb_t = delegate of IntPtr * IntPtr -> unit 

[<Struct>]
type pa_server_info = struct end
type pa_server_info_cb_t = delegate of IntPtr * nativeptr<pa_server_info> * IntPtr -> unit

[<Struct>]
[<StructLayout(LayoutKind.Sequential)>]
type pa_spawn_api = 
    val mutable prefork: Callback
    val mutable postfork: Callback 
    val mutable atfork: Callback

[<Struct>]
type pa_proplist = struct end

[<DllImport ("libpulse", EntryPoint="pa_mainloop_new",CallingConvention=CallingConvention.Cdecl)>]
extern IntPtr pa_mainloop_new ()

[<DllImport ("libpulse", EntryPoint="pa_mainloop_get_api",CallingConvention=CallingConvention.Cdecl)>]
extern IntPtr pa_mainloop_get_api (IntPtr a)

[<DllImport ("libpulse", EntryPoint="pa_context_new",CallingConvention=CallingConvention.Cdecl)>]
extern IntPtr pa_context_new (IntPtr a)

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

[<DllImport ("libpulse", EntryPoint="pa_context_get_server_info",CallingConvention=CallingConvention.Cdecl)>]
extern IntPtr pa_context_get_server_info (
    IntPtr c,
    pa_server_info_cb_t cb,
    IntPtr usedata)

