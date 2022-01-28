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
(* snd_mixer_selem_set_playback_volume_all(elem, volume * max / 100); *)

(* snd_mixer_close(handle); *)

open Ctypes
open Foreign

type snd_mixer_t = unit
let  snd_mixer_t : snd_mixer_t typ = void

(* int 	snd_mixer_open (snd_mixer_t **mixer, int mode) *)
let snd_mixer_open =
    foreign "snd_mixer_open" (ptr (ptr snd_mixer_t) @-> int @-> returning int)

(* int 	snd_mixer_attach (snd_mixer_t *mixer, const char *name) *)
let snd_mixer_attach =
    foreign "snd_mixer_attach" (ptr snd_mixer_t @-> string @-> returning int)
