type net_if = {
    name: string;
    rx: int64; (* Bytes *)
    tx: int64;
    dr: float;
    dt: float;
}

type net_info = {
    mutable intfs : (string*net_if) list
}

let net_info_i = {intfs=[("eth0", {name="eth0";rx=0L;tx=0L;dr=0.0;dt=0.0})]}
