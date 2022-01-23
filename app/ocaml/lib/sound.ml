open Ctypes

open Printf
open Alsa

let info () = 
    let card = "default" and
        selem_name = "Master" in
    let handle = from_voidp snd_mixer_t null in
    let s = snd_mixer_open (handle) in
    printf "%s, %s, %d" card selem_name s


