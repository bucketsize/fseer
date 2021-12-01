module Snd 

open System
open System.IO
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open FSharp.NativeInterop

type SndInfo = {
    vol: float32;
    muted: bool;
}

[<Struct>]
type pa_mainloop = struct end

[<Struct>]
type pa_mainloop_api = struct end

[<Struct>]
type pa_context = struct end

[<Struct>]
type pa_context_flags = 
    | PA_CONTEXT_NOFLAGS
    | PA_CONTEXT_NOAUTOSPAWN
    | PA_CONTEXT_NOFAIL

[<IsByRefLike; Struct>]
[<StructLayout(LayoutKind.Sequential)>]
type pa_spawn_api = 
    val prefork: IntPtr
    val postfork: IntPtr 
    val atfork: IntPtr

[<DllImport ("libpulse", SetLastError=true, EntryPoint="pa_mainloop_new",CallingConvention=CallingConvention.Cdecl)>]
extern pa_mainloop* pa_mainloop_new ()

[<DllImport ("libpulse", SetLastError=true, EntryPoint="pa_mainloop_get_api",CallingConvention=CallingConvention.Cdecl)>]
extern pa_mainloop_api* pa_mainloop_get_api (pa_mainloop& a)

[<DllImport ("libpulse", SetLastError=true, EntryPoint="pa_context_new",CallingConvention=CallingConvention.Cdecl)>]
extern pa_context* pa_context_new(pa_mainloop_api& a)

[<DllImport ("libpulse", SetLastError=true, EntryPoint="pa_context_connect",CallingConvention=CallingConvention.Cdecl)>]
extern int _pa_context_connect (
    pa_context& c,
    string server,
    pa_context_flags flags,
    pa_spawn_api& api)

let pa_context_connect () = 
    let mutable c = pa_context()
    let s = ""
    let f = pa_context_flags.PA_CONTEXT_NOFLAGS
    let mutable a = pa_spawn_api() 
    _pa_context_connect (&c, s, f, &a)

let isNativeNull (x:nativeptr<_>) = 
    let j = NativePtr.read x
    obj.ReferenceEquals (j, null)

let isNotNativeNull x = not (isNativeNull x)

let info () =
    let paMainLoopPtr = pa_mainloop_new()
    if isNotNativeNull paMainLoopPtr then
        printfn "pa main loop"
    else
        printfn "pa main loop failed"

    let paMainLoopApiPtr = pa_mainloop_get_api (NativePtr.toByRef paMainLoopPtr)    
    if isNotNativeNull paMainLoopApiPtr then
        printfn "pa main loop api"
    else
        printfn "pa main loop api failed"

    let paContextPtr = pa_context_new (NativePtr.toByRef paMainLoopApiPtr)
    if isNotNativeNull paContextPtr then
        printfn "pa context"
    else
        printfn "pa context failed"




    {vol=0f; muted=false}

