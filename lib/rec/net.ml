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

