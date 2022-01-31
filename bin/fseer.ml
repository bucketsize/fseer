open Fseer

let () =
    let writefn =
        if Array.length Sys.argv > 1 then
            match Sys.argv.(1) with
            | "lemonbar" -> Lemonbar_writer.write
            | _ -> Console_writer.write
        else
            Console_writer.write and
        () = Sound_proxy.info() |> ignore
    in
    while true do 
        let sysinfo: Fseerrec.Metrics.metrics = {
            cpuinfo = Cpu.info();
            cpufreq = Cpufreq.info();
            cputemp = Cputemp.info();
            meminfo = Mem.info();
            netinfo = Net.info();
            pwrinfo = Power.info();
            sndinfo = Sound_proxy.info();
        } in
        writefn sysinfo;
        flush_all ();
        Unix.sleep Consts.poll_interval;
    done
