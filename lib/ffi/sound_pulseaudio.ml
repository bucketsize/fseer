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

let info (m: Fseerrec.Metrics.metrics) zfn =
    if not pa_info0.context_connected then
        let () = Pulseaudio.connect_cb 
                    (fun status -> 
                        printf "oc> connected: %d\n" status;
                        pa_info0.context_connected <- true
                        ) and
            () = Pulseaudio.sink_cb 
                    (fun sink ->
                        printf "oc> sink: %s\n" sink;
                        let () = snd_info0.sink <- sink in
                        let () = m.snd_info <- snd_info0 in
                        ()
                        (* zfn m *)
                        ) and
            () = Pulseaudio.volume_cb 
                    (fun volume -> 
                        printf "oc> volume: %f\n" volume;
                        let () = snd_info0.volume <-
                                    (Int.of_float (volume *. 100.0)) in
                        let () = m.snd_info <- snd_info0 in
                        ()
                        (* zfn m *)
                        ) and
            () = Pulseaudio.muted_cb 
                    (fun muted ->
                        printf "oc> muted: %B\n" muted;
                        let () = snd_info0.muted <- muted in
                        let () = m.snd_info <- snd_info0 in
                        ()
                        (* zfn m *)
                        ) in
        Pulseaudio.connect ()
    else
        Pulseaudio.tick ()
    
