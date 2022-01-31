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

type snd_mixer_selem_regopt = unit
let  snd_mixer_selem_regopt : snd_mixer_selem_regopt typ = void
type snd_mixer_class_t = unit
let  snd_mixer_class_t : snd_mixer_class_t typ = void

(* int 	snd_mixer_selem_register (snd_mixer_t *mixer, struct snd_mixer_selem_regopt *options, snd_mixer_class_t **classp) *)
let snd_mixer_selem_register =
    foreign "snd_mixer_selem_register" (ptr snd_mixer_t 
        @-> ptr snd_mixer_selem_regopt
        @-> ptr (ptr snd_mixer_class_t)
        @-> returning int)

(* int 	snd_mixer_load (snd_mixer_t *mixer) *)
let snd_mixer_load =
    foreign "snd_mixer_load" (ptr snd_mixer_t @-> returning int)

type snd_mixer_selem_id_t = unit
let  snd_mixer_selem_id_t : snd_mixer_selem_id_t typ = void

(* let snd_mixer_selem_id_t_sizeof = *) 
(*     foreign "snd_mixer_selem_id_t_sizeof" (void @-> returning int) *)

let calloc = 
    foreign "calloc" (int @-> int @-> returning (ptr void))

(* void snd_mixer_selem_id_set_name(snd_mixer_selem_id_t *obj, const char *val); *)
let snd_mixer_selem_id_set_name = 
    foreign "snd_mixer_selem_id_set_name" (ptr snd_mixer_selem_id_t
        @-> string 
        @-> returning void)

(* void snd_mixer_selem_id_set_index(snd_mixer_selem_id_t *obj, unsigned int val); *)
let snd_mixer_selem_id_set_index = 
    foreign "snd_mixer_selem_id_set_index" (ptr snd_mixer_selem_id_t
        @-> int 
        @-> returning void)

type snd_mixer_elem_t = unit 
let  snd_mixer_elem_t : snd_mixer_elem_t typ = void 

(* snd_mixer_elem_t * 	snd_mixer_find_selem (snd_mixer_t *mixer, const snd_mixer_selem_id_t *id)  *)
let snd_mixer_find_selem = 
    foreign "snd_mixer_find_selem" (ptr snd_mixer_t 
        @-> ptr snd_mixer_selem_id_t
        @-> returning (ptr snd_mixer_elem_t)) 

(* int snd_mixer_selem_get_playback_volume_range 	( 	snd_mixer_elem_t *  	elem,
		long *  	min,
		long *  	max 
	) 	 *)
let snd_mixer_selem_get_playback_volume_range =
    foreign "snd_mixer_selem_get_playback_volume_range" (ptr snd_mixer_elem_t
        @-> ptr long
        @-> ptr long
        @-> returning int)
