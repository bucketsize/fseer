module Snd 

open System
open System.IO
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open FSharp.NativeInterop
open Pa2
open Util

type SndInfo = {
    vol: float32;
    muted: bool;
}

(*
let info () =
    let paMainLoopPtr = pa_mainloop_new()
    if isNaRefNull paMainLoopPtr
    then printfn "pa main loop"
    else () 

    let paMainLoopApiPtr = pa_mainloop_get_api (NativePtr.toByRef paMainLoopPtr)    
    if isNaRefNull paMainLoopApiPtr
    then printfn "pa main loop api"
    else () 

    let paContextPtr = pa_context_new (NativePtr.toByRef paMainLoopApiPtr)
    if isNaRefNull paContextPtr
    then printfn "pa context"
    else ()

    let flags = pa_context_flags.PA_CONTEXT_NOAUTOSPAWN
    let mutable a = pa_spawn_api() 
    let c = pa_context_connect (NativePtr.toByRef paContextPtr, null, flags, &a)
    if c < 0
    then printfn "pa context connect failed: %d" c
    else ()

    let mutable paInfoCb = pa_server_info_cb_t()
    let mutable paUdata = pa_userdata()
    let paOpPtr = pa_context_get_server_info (NativePtr.toByRef paContextPtr, &paInfoCb, &paUdata)
    if isNaRefNull paOpPtr
    then printfn "pa op"
    else ()

    let paOp = NativePtr.read paOpPtr
    printfn "index=%d, n_used=%d" paOp.index paOp.n_used

    {vol=0f; muted=false}
*)


let prefork () =
    printfn "prefork"
let postfork () =
    printfn "postfork"
let atfork () =
    printfn "atfork"
let paContextNotifyCb (c:IntPtr) (userdata:IntPtr) = 
    printfn "paContextNotify ?"
let paServerInfoCb (c:IntPtr) (serverInfo:nativeptr<pa_server_info>) (userdata:IntPtr) = 
    printfn "paServerInfo ?"
let info () =
    let paMainLoopPtr = pa_mainloop_new()
    if paMainLoopPtr = IntPtr.Zero
    then printfn "pa_mainloop_new failed"
    else () 

    let paMainLoopApiPtr = pa_mainloop_get_api (paMainLoopPtr)    
    if paMainLoopApiPtr = IntPtr.Zero
    then printfn "pa_mainloop_get_api failed"
    else () 

    let paContextPtr = pa_context_new (paMainLoopApiPtr)
    if paContextPtr = IntPtr.Zero 
    then printfn "pa_context_new failed"
    else ()

    let flags = pa_context_flags.PA_CONTEXT_NOAUTOSPAWN
    let mutable apis = pa_spawn_api ()
    apis.prefork <- Callback(prefork)
    apis.postfork <- Callback(postfork)
    apis.atfork <- Callback(atfork)
    
    let c = pa_context_connect (paContextPtr, null, flags, &apis)
    if c < 0
    then printfn "pa_context_connect failed: %d" c
    else printfn "pa_context_connect success: %d" c 

    pa_context_set_state_callback (
        paContextPtr,
        pa_context_notify_cb_t(paContextNotifyCb),
        IntPtr.Zero)


    // TODO: try to run mainloop after this before getting server info


    let mutable paInfoCb = pa_server_info_cb_t(paServerInfoCb)
    let paOpPtr = pa_context_get_server_info (paContextPtr, paInfoCb, IntPtr.Zero)
    if paOpPtr = IntPtr.Zero
    then printfn "pa_context_get_server_info failed"
    else ()

    //let paOp = NativePtr.read paOpPtr
    //printfn "index=%d, n_used=%d" paOp.index paOp.n_used

    {vol=0f; muted=false}
