open System.Threading
open Metrics

[<EntryPoint>]
let main argv =
    let sndinfo = Snd.info();
    while true do
        let sysinfo: Metrics = {
            cpuinfo = Cpu.info();
            cpufreq = CpuFreq.info();
            cputemp = CpuTemp.info();
            meminfo = Mem.info();
            dskinfo = Disks.info();
            pwrinfo = Power.info();
            netinfo = Net.info();
        }
        ConsoleWriter.write sysinfo
        LemonbarWriter.write sysinfo
        Thread.Sleep(2000)
    0 
