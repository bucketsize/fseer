module Snd 

open System
open System.IO
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open FSharp.NativeInterop
open Pa
open Util

let mutable GcHs = Map []
let gcSkip<'a> (name:string) (o:'a) = 
    let o' = o :> obj
    let gcH = GCHandle.Alloc o'
    GcHs <- GcHs.Add (name, o')
    o

let naNullRef<'a when 'a : unmanaged> () = 
        NativePtr.ofNativeInt<'a> IntPtr.Zero

type SndInfo = { 
    mutable vol: float32;
    mutable muted: bool;
}

type PaContext = {
    mutable paContext: nativeptr<pa_context>;
    mutable paMainLoop: nativeptr<pa_mainloop>;
    mutable paConnected: bool;
}

let mutable paContext =
    {
        paContext  = naNullRef<pa_context> ();
        paMainLoop = naNullRef<pa_mainloop> ();
        paConnected=false; 
    }
let mutable sndInfo =
    {
        vol=0f;
        muted=false
    }

let prefork () =
    printfn "prefork"
let postfork () =
    printfn "postfork"
let atfork () =
    printfn "atfork"
let paSinkInfo 
  (c: pa_context nativeptr)
  (sIp: IntPtr)
  (eol: int)
  (userdata: nativeint) = 
    printfn "paSinkInfo"
    if eol = 0
    then
        let mutable sI = Marshal.PtrToStructure<pa_sink_info>(sIp)
        let vols =
            sI.volume.values 
                |> (Array.fold (fun s x -> s + ", " + x.ToString()) "")
        printfn "paSinkInfo: %s [%s], eol=%d, driver=%s, muted=%d, state=%s, flags=%s, chnls=%d, Bvol=%d, vol=%s" 
            sI.name
            sI.description
            eol
            sI.driver
            sI.mute
            (sI.state.ToString())
            (sI.flags.ToString())
            sI.volume.channels
            sI.base_volume
            vols
        printfn "%A" sI 
        printfn "paSinkInfo::debug: pa_cvolume_valid=%d" (pa_cvolume_valid(&sI.volume))
     
        (*
        let paPropsStr = pa_proplist_to_string_sep (sI.proplist, "; ")
        printfn "paSinkInfo::debug: pa_proplist=%s, tostring=%s"
            paPropsStr (sI.proplist.ToString()) 
        *)

        let pVolAvg = pa_cvolume_avg (&sI.volume)
        printfn "paSinkInfo::debug: pa_cvolume_avg=%d" pVolAvg

        let mutable strCVol = "" 
        let strCVol1 = pa_cvolume_snprint (strCVol, 320, &sI.volume)
        printfn "paSinkInfo::debug: pa_cvolume=%s" strCVol1
        
    else
        ()

// Name: alsa_output.platform-bcm2835_audio.stereo-fallback.3
//       alsa_output.platform-bcm2835_audio.stereo-fallback.3
let querySink
  (c: pa_context nativeptr)
  (sI: pa_server_info byref)
  (userdata: nativeint) = 
    let paSinkInfoCb =
        (*
        (pa_sink_info_cb_t
            (fun 
                (a: pa_context nativeptr)
                (b: IntPtr)
                (e: int) 
                (d: nativeint) -> paSinkInfo a b e d) )
         *)
        (pa_sink_info_cb_t (paSinkInfo))
        // |> (gcSkip<pa_sink_info_cb_t> "paSinkInfoCb") 
    let paOpPtr =
        pa_context_get_sink_info_by_name (
            paContext.paContext,
            sI.default_sink_name,
            paSinkInfoCb,
            userdata)
    //let paOpPtr = pa_context_get_sink_info_list (c, paSinkInfoCb, userdata)
    if isNaRefNull paOpPtr
    then printfn "pa_context_get_sink_info_by_name failed"
    else printfn "pa_context_get_sink_info_by_name"

let paServerInfo
  (c: pa_context nativeptr)
  (sI: pa_server_info byref)
  (userdata: nativeint) = 
    printfn "paServerInfo: %s %s / %s on %s, dsink=%s, dsource=%s" 
        sI.server_name 
        sI.server_version 
        sI.user_name
        sI.host_name
        sI.default_sink_name
        sI.default_source_name
    querySink c &sI userdata

let queryServer
  (c: pa_context nativeptr)
  (userdata: nativeint) = 
    let paServerInfoCb =
        (pa_server_info_cb_t 
            (fun 
                (a: pa_context nativeptr)
                (b: pa_server_info byref)
                (d: nativeint) -> paServerInfo a &b d)) 
        |> (gcSkip<pa_server_info_cb_t> "paServerInfoCb")
    let paOpPtr =
        pa_context_get_server_info (
            paContext.paContext,
            paServerInfoCb,
            userdata)
    if isNaRefNull paOpPtr
    then printfn "pa_context_get_server_info failed"
    else printfn "pa_context_get_server_info"
    
let paContextNotify
  (c: pa_context nativeptr)
  (userdata: nativeint) = 
    let paState = pa_context_get_state (paContext.paContext)
    printfn "paContextNotify::paState: %s" (paState.ToString())
    if paState = pa_context_state_t.PA_CONTEXT_READY
    then
        paContext.paConnected <- true
        queryServer c userdata
    else ()

let paConnect () =
    let paMainLoopPtr = pa_mainloop_new()
    if isNaRefNull paMainLoopPtr
    then printfn "pa_mainloop_new failed"
    else () 
    paContext.paMainLoop <- paMainLoopPtr

    let paMainLoopApiPtr = pa_mainloop_get_api (paMainLoopPtr)    
    if isNaRefNull paMainLoopApiPtr
    then printfn "pa_mainloop_get_api failed"
    else () 

    let paContextPtr = pa_context_new (paMainLoopApiPtr, "fseer")
    if isNaRefNull paContextPtr
    then printfn "pa_context_new failed"
    else ()
    paContext.paContext  <- paContextPtr

    let paContextNotifyCb =
        (pa_context_notify_cb_t 
            (fun 
                (a: pa_context nativeptr)
                (b: nativeint) -> paContextNotify a b)) 
        |> (gcSkip<pa_context_notify_cb_t> "paContextNotifyCb")
    pa_context_set_state_callback (
        paContextPtr,
        paContextNotifyCb,
        IntPtr.Zero)

    let flags = pa_context_flags.PA_CONTEXT_NOAUTOSPAWN
    let mutable apis = pa_spawn_api (void_cb_t(prefork), void_cb_t(postfork), void_cb_t(atfork))
    
    let c = pa_context_connect (paContextPtr, null, flags, &apis)
    if c < 0
    then printfn "pa_context_connect failed: %d" c
    else printfn "pa_context_connect: %d" c 

    paContext

let info () = 
    let block = 0
    let mutable retVal = 0
    if not paContext.paConnected
    then
        paContext <- paConnect ()
        while not paContext.paConnected do
            let s = pa_mainloop_iterate (paContext.paMainLoop, block, &retVal)
            ()
    else
        let s = pa_mainloop_iterate (paContext.paMainLoop, block, &retVal)
        ()
    sndInfo

