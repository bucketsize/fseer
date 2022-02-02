open Fseer
open Fseerout
open Printf

let () =
    let writefn =
        if Array.length Sys.argv > 1 then
            match Sys.argv.(1) with
            | "lemonbar" -> Lemonbar_writer.write
            | _ -> Console_writer.write
        else
            Console_writer.write in
    let noti_fn (m: Fseerrec.Metrics.metrics) =
        (writefn m) in
    let g_metrics: Fseerrec.Metrics.metrics = {
        cpu_info = Fseerrec.Cpu.cpu_info_i;
        cpu_freq = Fseerrec.Cpu_freq.cpu_freq_i;
        cpu_temp = Fseerrec.Cpu_temp.cpu_temp_i;
        mem_info = Fseerrec.Mem.mem_info_i;
        net_info = Fseerrec.Net.net_info_i;
        pwr_info = Fseerrec.Power.power_info_i;
        snd_info = Fseerrec.Sound.sound_info_i;
    } in
    while true do 
        let () = Cpu.info g_metrics noti_fn and
            () = Cpu_freq.info g_metrics noti_fn and
            () = Cpu_temp.info g_metrics noti_fn and
            () = Mem.info g_metrics noti_fn and
            () = Net.info g_metrics noti_fn and
            () = Power.info g_metrics noti_fn and
            () = Sound_proxy.info g_metrics noti_fn in
        let () = writefn g_metrics in
        Unix.sleep Consts.poll_interval
    done
