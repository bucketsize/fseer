open Fseer

let update_fn writefn (cb: ((unit -> unit) option)) =
    (fun () -> 
        Printf.printf "oc> callback called\n";
        let sysinfo: Fseerrec.Metrics.metrics = {
            cpuinfo = Cpu.info(cb);
            cpufreq = Cpu_freq.info(cb);
            cputemp = Cpu_temp.info(cb);
            meminfo = Mem.info(cb);
            netinfo = Net.info(cb);
            pwrinfo = Power.info(cb);
            sndinfo = Sound_proxy.info(cb);
        } in
        let () = writefn sysinfo in
        flush_all ())

let () =
    let writefn =
        if Array.length Sys.argv > 1 then
            match Sys.argv.(1) with
            | "lemonbar" -> Lemonbar_writer.write
            | _ -> Console_writer.write
        else
            Console_writer.write  in
    let update_cb = Some (update_fn writefn None) in
    while true do 
        let poll = update_fn writefn update_cb in
        let () = poll () in
        Unix.sleep Consts.poll_interval
    done
