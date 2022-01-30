open Sound
open Printf

let snd_info0 = {
    card = "default";
    mixer = "Master";
    vol = 0;
} 

type pa_info = {
    mutable context_connected: bool;
}

let pa_info0 = {context_connected=false;}

let info () =
    if not pa_info0.context_connected then
        let () = Ocaml_pulseaudio.connect_cb 
                    (fun status -> 
                        let () = printf "oc> connected: %d\n" status and
                            () = pa_info0.context_connected <- true in
                        ()
                    ) and
            () = Ocaml_pulseaudio.sink_cb 
                    (fun sink -> printf "oc> sink: %s\n" sink) and
            () = Ocaml_pulseaudio.volume_cb 
                    (fun volume -> printf "oc> volume: %f\n" volume) and
            () = Ocaml_pulseaudio.muted_cb 
                    (fun muted -> printf "oc> muted: %B\n" muted) in
        let () = Ocaml_pulseaudio.connect () in
        snd_info0
    else
        let () = Ocaml_pulseaudio.tick () in
        snd_info0
    
