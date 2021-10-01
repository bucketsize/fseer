module Disks

open System
open System.IO

let parts =
    [for i in ["a";"b";"c";"d";"e";"f";"g"] do
        for j = 0 to 3 do
            yield ((sprintf "sd%s%d" i j),
                (sprintf "/sys/block/sd%s%d/stat" i j))
            yield ((sprintf "hd%s%d" i j),
                (sprintf "/sys/block/hd%s%d/stat" i j))

     for i = 0 to 3 do
        for j = 0 to 3 do
            yield ((sprintf "mmcblk%dp%d" i j),
                (sprintf "/sys/block/mmcblk%d/mmcblk%dp%d/stat" i i j))
    ]
    |> List.filter (fun (k,v) -> File.Exists(v))

let mutable stats = parts |> List.map (fun (k,v) -> (k,(0,0))) |> Map.ofList

let info(writer) =
    parts 
    |> List.iter (fun (k,v) -> 
        let ss = Util.readLine(v).Split(" ") 
                    |> Seq.filter (fun x -> x <> "")
                    |> Seq.toList
        //printfn "> %A" ss            
        let r2,w2 = Int32.Parse(ss.[0]), Int32.Parse(ss.[4])
        let r1,w1 = stats.[k]            
        let dr, dw = r2-r1, w2-w1
        stats <- stats.Add(k, (r2, w2))
        writer(k, dr, dw)
    )

