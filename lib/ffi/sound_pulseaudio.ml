open Printf
open Fseerrec.Sound

let snd_info0 = {
    card = "default";
    mixer = "Master";
    sink = "?";
    volume = 0;
    muted = false;
} 

type pa_info = {
    mutable context_connected: bool;
}

let pa_info0 = {context_connected=false;}

let info zfn =
    if not pa_info0.context_connected then
        let () = Pulseaudio.connect_cb 
                    (fun status -> 
                        let () = pa_info0.context_connected <- true in
                        printf "oc> connected: %d\n" status) and
            () = Pulseaudio.sink_cb 
                    (fun sink ->
                        let () = snd_info0.sink <- sink in
                        let () = match zfn with
                                    | Some fn -> fn () 
                                    | None -> () in
                        printf "oc> sink: %s\n" sink) and
            () = Pulseaudio.volume_cb 
                    (fun volume -> 
                        let () = snd_info0.volume <-
                                    (Int.of_float (volume *. 100.0)) in
                        let () = match zfn with
                                    | Some fn -> fn () 
                                    | None -> () in
                        printf "oc> volume: %f\n" volume) and
            () = Pulseaudio.muted_cb 
                    (fun muted ->
                        let () = snd_info0.muted <- muted in
                        printf "oc> muted: %B\n" muted) in
        let () = Pulseaudio.connect () in
        snd_info0
    else
        let () = Pulseaudio.tick () in
        snd_info0
    
