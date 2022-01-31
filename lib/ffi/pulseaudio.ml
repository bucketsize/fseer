external connect_cb:
    (int -> unit) -> unit = "caml_pa_connect_cb"
external sink_cb:
    (string -> unit) -> unit = "caml_pa_sink_cb"
external volume_cb:
    (float -> unit) -> unit = "caml_pa_volume_cb"
external muted_cb:
    (bool -> unit) -> unit = "caml_pa_muted_cb"
external connect:
    unit -> unit = "caml_pa_connect"
external tick:
    unit -> unit = "caml_pa_tick"
