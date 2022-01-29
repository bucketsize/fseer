open Ctypes

open Printf
open Alsa
open Pulseaudio

let alsainfo () = 
    let card = "default" in
    let handle_ref = allocate (ptr snd_mixer_t) (from_voidp snd_mixer_t null) in
    let s1 = snd_mixer_open handle_ref 0 in
    let handle = !@ handle_ref in
    let () = printf "open: %d\n" s1 in
    let s2 = snd_mixer_attach handle card in
    let () = printf "attach: %d\n" s2 in
    ()

let pulseinfo () = 
    let paMainloop = pa_mainloop_new () in
    let paMainloopApi = pa_mainloop_get_api paMainloop in
    let paContext = pa_context_new paMainloopApi "ocampulse" in
    if (is_null paContext) then
        printf "pa context failed\n"
    else
        printf "pa context done\n"

let info () = 
    let () = alsainfo () in 
    pulseinfo ()
