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

type LogSpec = {
    level: string;
}

let logSpec = {level = "debug"}

let printfv<'a> (level: string) (format: Printf.TextWriterFormat<'a>) = 
    Printf.kprintf (printfn "[%s][%A] %s" level System.DateTime.Now format 

let log_debug =
    if logSpec.level = "debug"
    then printfn 
    else printfv


let naNullRef<'a when 'a : unmanaged> () = 
        NativePtr.ofNativeInt<'a> IntPtr.Zero

type Decibels = float
type SndInfo = { 
    mutable source: string
    mutable sink: string
    mutable port: string
    mutable volume: Decibels; 
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
        source = "";
        sink = "";
        port = "";
        volume =  0 |> float;
        muted = false;
    }

let prefork () =
    log_debug "prefork"
let postfork () =
    log_debug "postfork"
let atfork () =
    log_debug "atfork"
let paSinkInfo 
  (c: pa_context nativeptr)
  (sIp: IntPtr)
  (eol: int)
  (userdata: IntPtr) = 
    log_debug "paSinkInfo"
    if eol = 0
    then
        let mutable sI = Marshal.PtrToStructure<pa_sink_info>(sIp)
        log_debug "%s" (sI.ToString())
        let vols =
            sI.volume.values 
        let vols_dB =
            sI.volume.values
            |> Array.map (fun x -> pa_sw_volume_to_dB (x))
        let vols_linear =
            sI.volume.values
            |> Array.map (fun x -> pa_sw_volume_to_linear (x))
        log_debug "paSinkInfo.volumes: %d \n\t%A \n\t%A \n\t%A"
            sI.base_volume
            vols
            vols_dB
            vols_linear
        log_debug "paSinkInfo::debug: pa_cvolume_valid=%d" (pa_cvolume_valid(&sI.volume))

        let paPropsStr =
            if isNotNull sI.proplist
            then pa_proplist_to_string (sI.proplist)
            else "null"

        log_debug "paSinkInfo::debug: pa_proplist=\n%s" paPropsStr

        let pVolAvg = pa_cvolume_avg (&sI.volume)
    
        sndInfo.volume <- (pa_sw_volume_to_dB (pVolAvg)) |> float
        log_debug "paSinkInfo::debug: pa_cvolume_avg=%A" pVolAvg
        ()
        
    else
        ()

// Name: alsa_output.platform-bcm2835_audio.stereo-fallback.3
//       alsa_output.platform-bcm2835_audio.stereo-fallback.3
let querySink
  (c: pa_context nativeptr)
  (sI: pa_server_info byref)
  (userdata: IntPtr) = 
    let paSinkInfoCb =
        (pa_sink_info_cb_t (paSinkInfo))
        |> (gcSkip<pa_sink_info_cb_t> "paSinkInfoCb") 
    let paOpPtr =
        pa_context_get_sink_info_by_name (
            paContext.paContext,
            sI.default_sink_name,
            paSinkInfoCb,
            userdata)
    //let paOpPtr = pa_context_get_sink_info_list (c, paSinkInfoCb, userdata)
    if isNull paOpPtr
    then log_debug "pa_context_get_sink_info_by_name failed"
    else log_debug "pa_context_get_sink_info_by_name"

let paServerInfo
  (c: pa_context nativeptr)
  (sIp: IntPtr)
  (userdata: IntPtr) = 
    let mutable sI = Marshal.PtrToStructure<pa_server_info> (sIp)
    log_debug "%s" (sI.ToString())

    sndInfo.source <- sI.default_source_name
    sndInfo.sink <- sI.default_sink_name

    querySink c &sI userdata

let queryServer
  (c: pa_context nativeptr)
  (userdata: IntPtr) = 
    let paServerInfoCb =
        (pa_server_info_cb_t paServerInfo)
        |> (gcSkip<pa_server_info_cb_t> "paServerInfoCb")
    let paOpPtr =
        pa_context_get_server_info (
            paContext.paContext,
            paServerInfoCb,
            userdata)
    if isNull paOpPtr
    then log_debug "pa_context_get_server_info failed"
    else log_debug "pa_context_get_server_info"
    
let paContextNotify
  (c: pa_context nativeptr)
  (userdata: IntPtr) = 
    let paState = pa_context_get_state (paContext.paContext)
    log_debug "paContextNotify::paState: %s" (paState.ToString())
    if paState = pa_context_state_t.PA_CONTEXT_READY
    then
        paContext.paConnected <- true
        queryServer c userdata
    else ()

let paConnect () =
    let paMainLoopPtr = pa_mainloop_new()
    if isNull paMainLoopPtr
    then log_debug "pa_mainloop_new failed"
    else () 
    paContext.paMainLoop <- paMainLoopPtr

    let paMainLoopApiPtr = pa_mainloop_get_api (paMainLoopPtr)    
    if isNull paMainLoopApiPtr
    then log_debug "pa_mainloop_get_api failed"
    else () 

    let paContextPtr = pa_context_new (paMainLoopApiPtr, "fseer")
    if isNull paContextPtr
    then log_debug "pa_context_new failed"
    else ()
    paContext.paContext  <- paContextPtr

    let paContextNotifyCb =
        (pa_context_notify_cb_t 
            (fun 
                (a: pa_context nativeptr)
                (b: IntPtr) -> paContextNotify a b)) 
        |> (gcSkip<pa_context_notify_cb_t> "paContextNotifyCb")

    let userdata = IntPtr.Zero
    pa_context_set_state_callback (
        paContextPtr,
        paContextNotifyCb,
        userdata)

    let flags = pa_context_flags.PA_CONTEXT_NOAUTOSPAWN
    let mutable apis = pa_spawn_api (void_cb_t(prefork), void_cb_t(postfork), void_cb_t(atfork))
    
    let c = pa_context_connect (paContextPtr, null, flags, &apis)
    if c < 0
    then log_debug "pa_context_connect failed: %d" c
    else log_debug "pa_context_connect: %d" c 

    paContext

let info () = 
    let mutable retVal = 0
    if not paContext.paConnected
    then
        paContext <- paConnect ()
        while not paContext.paConnected do
            let s = pa_mainloop_iterate (paContext.paMainLoop, 0, &retVal)
            ()
    else
        let s = pa_mainloop_iterate (paContext.paMainLoop, 0, &retVal)
        ()
    sndInfo

