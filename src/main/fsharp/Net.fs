module Net

open System
open System.IO
open System.Text.RegularExpressions

type Ni = {
    intf: string;
    rx: Int32; // Bytes
    tx: Int32;
}
type NetInfo = {
    intfs : List<Ni>
}

let info () =
    let r = new Regex("(\d+)") 
    let f = (File.ReadLines  @"/proc/net/dev"
        |> Seq.filter (fun x -> not ((x.Contains "Inter") || (x.Contains "face")))
        |> Seq.map (fun x -> 
            let xs = x.Split ":"
            let nd = xs.[0].Trim()
            let nv = (r.Matches xs.[1] 
                |> Seq.map (fun x -> Int32.Parse(x.Value))
                |> Seq.toList)
            {intf = nd;  rx = nv.Item 0; tx = nv.[1]})
        |> Seq.toList)
    {intfs = f}

