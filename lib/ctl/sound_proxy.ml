(* TODO via pulse api *)
let vol_up     snd sval =
    Sys.command "pactl set-sink-volume @DEFAULT_SINK@ +10%" |> ignore;
    ()
let vol_down   snd sval =
    Sys.command "pactl set-sink-volume @DEFAULT_SINK@ -10%" |> ignore;
    ()
let vol_mute   snd sval =
    Sys.command "pactl set-sink-mute   @DEFAULT_SINK@ toggle" |> ignore;
    ()
let vol_unmute snd sval =
    Sys.command "pactl set-sink-mute   @DEFAULT_SINK@ toggle" |> ignore;
    ()
