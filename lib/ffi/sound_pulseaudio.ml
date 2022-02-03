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
    mutable cb_registered: bool;
    mutable context_requested: bool;
    mutable context_connected: bool;
}

let pa_info0 = {
    cb_registered=false;
    context_requested=false;
    context_connected=false;
}

let try_pa_cb_register (m: Fseerrec.Metrics.metrics) zfn =
    if (not pa_info0.cb_registered) then 
        let () = Pulseaudio.connect_cb 
                    (fun status -> 
                        printf "oc> pa context ready: %d\n" status;
                        pa_info0.context_connected <- true
                        ) and
            () = Pulseaudio.sink_cb 
                    (fun sink ->
                        printf "oc> pa sink: %s\n" sink;
                        let () = snd_info0.sink <- sink in
                        let () = m.snd_info <- snd_info0 in
                        ()
                        (* zfn m *)
                        ) and
            () = Pulseaudio.volume_cb 
                    (fun volume -> 
                        printf "oc> pa volume: %f\n" volume;
                        let () = snd_info0.volume <-
                                    (Int.of_float (volume *. 100.0)) in
                        let () = m.snd_info <- snd_info0 in
                        ()
                        (* zfn m *)
                        ) and
            () = Pulseaudio.muted_cb 
                    (fun muted ->
                        printf "oc> pa muted: %B\n" muted;
                        let () = snd_info0.muted <- muted in
                        let () = m.snd_info <- snd_info0 in
                        ()
                        (* zfn m *)
                        ) and 
            () = pa_info0.cb_registered <- true in
        printf "oc> pa callbacks registered\n"
    else
        ()

let info (m: Fseerrec.Metrics.metrics) zfn =
    try_pa_cb_register m zfn;
    if (not pa_info0.context_requested) then
        let () = Pulseaudio.connect () and
            () = pa_info0.context_requested <- true in
        ()
    else
        Pulseaudio.tick ()
    
