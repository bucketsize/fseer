open Ctypes

open Printf
open Sound
open Ctype_alsa

let alsainfo () =
(* long min, max; *)
(* snd_mixer_t *handle; *)
(* snd_mixer_selem_id_t *sid; *)
(* const char *card = "default"; *)
(* const char *selem_name = "Master"; *)

(* snd_mixer_open(&handle, 0); *)
(* snd_mixer_attach(handle, card); *)
(* snd_mixer_selem_register(handle, NULL, NULL); *)
(* snd_mixer_load(handle); *)

(* snd_mixer_selem_id_alloca(&sid); *)
(* snd_mixer_selem_id_set_index(sid, 0); *)
(* snd_mixer_selem_id_set_name(sid, selem_name); *)
(* snd_mixer_elem_t* elem = snd_mixer_find_selem(handle, sid); *)

(* snd_mixer_selem_get_playback_volume_range(elem, &min, &max); *)
    let sndinfo = {card="default"; mixer="Master"; vol=0} in
    let handle_ref = allocate (ptr snd_mixer_t) (from_voidp snd_mixer_t null) in
    snd_mixer_open handle_ref 0
    |> (fun pre -> 
        if pre = 0 then 
            let handle = !@ handle_ref in
            (snd_mixer_attach handle sndinfo.card, Some handle)
        else (pre, None))
    |> (fun (pre,handle) -> 
        if pre = 0 then
            let mc_ref = allocate (ptr snd_mixer_class_t) (from_voidp snd_mixer_class_t null) in
            match handle with
            | Some h ->
                (snd_mixer_selem_register h null mc_ref, handle)
            | None -> (2, None)
        else (pre, None))
    |> (fun (pre,handle) ->
        if pre = 0 then
            match handle with
            | Some h ->
                (snd_mixer_load h, handle)
            | None -> (2, None)
        else (pre, None))
    |> (fun (pre,handle) ->
        if pre = 0 then
            let ms_ref = allocate (ptr snd_mixer_selem_id_t) (from_voidp snd_mixer_selem_id_t null) in
            let ms_handle = !@ ms_ref in
            let tmp1 = calloc 1 (sizeof snd_mixer_selem_id_t) in
            let () = ms_handle <-@ (!@ tmp1) in
            (pre, handle, Some ms_handle)
        else (pre, None, None))
    |> (fun (pre,handle,ms_handle) ->
        let ms_handle_valid = not (is_null (Option.get ms_handle)) in
        let () = printf "ms_handle_valid: %b" ms_handle_valid in
        if (pre = 0) && ms_handle_valid then
            let () = snd_mixer_selem_id_set_index (Option.get ms_handle) 0 in
            (0, handle, ms_handle)           
        else (2, None, None))
    |> (fun (pre,_,_) -> 
        if pre = 0 then
            printf "asound connected\n"
        else
            printf "asound connect failed\n")
            
let info () = 
    alsainfo ()
