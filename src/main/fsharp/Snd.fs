module Snd 

open System
open System.IO
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open FSharp.NativeInterop
open Pa
open Util

type SndInfo = {
    mutable vol: float32;
    mutable muted: bool;
}

type PaContext = {
    paContext: IntPtr;
    paMainLoop: IntPtr;
}

let mutable GcHandles = []
let gcSkip o = 
    let GCh = GCHandle.Alloc o
    GcHandles <- List.Cons (GCh, GcHandles) 
    o

module Cb =
    let prefork () =
        printfn "prefork"
    let postfork () =
        printfn "postfork"
    let atfork () =
        printfn "atfork"
    let paServerInfo (c:IntPtr) (sI:inref<pa_server_info>) (userdata:IntPtr) = 
        printfn "paServerInfo: %s %s on %s, dsink=%s, dsource=%s" 
            sI.server_name 
            sI.server_version 
            sI.host_name
            sI.default_sink_name
            sI.default_source_name
    let paContextNotify (c:IntPtr) (userdata:IntPtr) = 
        let paState = pa_context_get_state (c)
        printfn "paContextNotify::paState: %s" (paState.ToString())
        if paState = pa_context_state_t.PA_CONTEXT_READY
        then
            let paServerInfoCb = gcSkip (pa_server_info_cb_t(fun c sI userdata -> (paServerInfo c &sI userdata)))
            let paOpPtr = pa_context_get_server_info (c, paServerInfoCb, IntPtr.Zero)
            if paOpPtr = IntPtr.Zero
            then printfn "pa_context_get_server_info failed"
        else ()

module NaCb =
    let paContextNotify = gcSkip (pa_context_notify_cb_t(Cb.paContextNotify))
    let prefork = Callback(Cb.prefork)
    let postfork =  Callback(Cb.postfork)
    let atfork = Callback(Cb.atfork)

let paConnect () =
    let paMainLoopPtr = pa_mainloop_new()
    if paMainLoopPtr = IntPtr.Zero
    then printfn "pa_mainloop_new failed"
    else () 

    let paMainLoopApiPtr = pa_mainloop_get_api (paMainLoopPtr)    
    if paMainLoopApiPtr = IntPtr.Zero
    then printfn "pa_mainloop_get_api failed"
    else () 

    let paContextPtr = pa_context_new (paMainLoopApiPtr, "fseer")
    if paContextPtr = IntPtr.Zero 
    then printfn "pa_context_new failed"
    else ()

    pa_context_set_state_callback (
        paContextPtr,
        NaCb.paContextNotify,
        IntPtr.Zero)

    let flags = pa_context_flags.PA_CONTEXT_NOAUTOSPAWN
    let mutable apis = pa_spawn_api (Callback(Cb.prefork), Callback(Cb.postfork), Callback(Cb.atfork))
    
    let c = pa_context_connect (paContextPtr, null, flags, &apis)
    if c < 0
    then printfn "pa_context_connect failed: %d" c
    else printfn "pa_context_connect ...: %d" c 

    {paContext=paContextPtr; paMainLoop=paMainLoopPtr}

let PaContext = paConnect ()
let mutable PaInfo = {vol=0f; muted=false}

let info () = 
    let block = 0
    let mutable retVal = 0
    let s = pa_mainloop_iterate (PaContext.paMainLoop, block, &retVal)
    printfn "pa_mainloop_iterate: %d, %d" s retVal
    PaInfo

