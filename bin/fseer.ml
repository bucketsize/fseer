open Fseermon
open Fseerout
open Printf

let g_metrics: Fseerrec.Metrics.metrics = {
    cpu_info = Fseerrec.Cpu.cpu_info_i;
    cpu_freq = Fseerrec.Cpu_freq.cpu_freq_i;
    cpu_temp = Fseerrec.Cpu_temp.cpu_temp_i;
    mem_info = Fseerrec.Mem.mem_info_i;
    net_info = Fseerrec.Net.net_info_i;
    pwr_info = Fseerrec.Power.power_info_i;
    snd_info = Fseerrec.Sound.sound_info_i;
}

let cmd_mon scmd = 
    let writefn =
        match scmd with
        | "lemonbar" -> Lemonbar_writer.write
        | _          -> Console_writer.write in
    let noti_fn (m: Fseerrec.Metrics.metrics) =
        (writefn m) in
    while true do 
        let () = Cpu.info g_metrics noti_fn and
            () = Cpu_freq.info g_metrics noti_fn and
            () = Cpu_temp.info g_metrics noti_fn and
            () = Mem.info g_metrics noti_fn and
            () = Net.info g_metrics noti_fn and
            () = Power.info g_metrics noti_fn and
            () = Sound_proxy.info g_metrics noti_fn in
        let () = writefn g_metrics in
        Unix.sleep Fseer.Consts.poll_interval
    done

let cmd_ctl scmd sval = 
    match scmd with
    | "vol_up" -> Fseerctl.Sound_proxy.vol_up g_metrics.snd_info sval
    | "vol_down" -> Fseerctl.Sound_proxy.vol_down g_metrics.snd_info sval
    | "vol_mute" -> Fseerctl.Sound_proxy.vol_mute g_metrics.snd_info sval
    | "vol_unmute" -> Fseerctl.Sound_proxy.vol_unmute g_metrics.snd_info sval
    | _        -> printf "unknown cmd"

let rec cmd_ctd scmd sval =
    let () = 
        match Stdio.In_channel.input_line Stdio.stdin with
        | Some line -> 
            let ss = String.split_on_char ' ' line 
                        |> List.filter (fun x -> x != "") in
            (* let () = printf "%d\n" (List.length ss) in *)
            (match ss with
               | x :: y :: _ -> 
                    cmd_ctl x y;
                    printf "200\n"
               | x :: [] ->
                    printf "400\n"
               | [] ->
                    printf "400\n"
            );
            flush stdout
        | None -> ()
    in
    cmd_ctd scmd sval

let () =
    if Array.length Sys.argv > 2 then
        match Sys.argv.(1) with
        | "mon" -> cmd_mon Sys.argv.(2)
        | "ctl" -> cmd_ctl Sys.argv.(2) Sys.argv.(3)
        | "ctd" -> cmd_ctd Sys.argv.(2) Sys.argv.(3)
        | _     -> printf "unknown cmd"
    else
        printf "expect atleast 3 params: cmd -> scmd -> sval"
