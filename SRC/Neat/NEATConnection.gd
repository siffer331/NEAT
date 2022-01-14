class_name NEATConnection

var innovation_id: int
var in_id: int
var out_id: int
var enabled: bool
var recurent: bool
var weight: float
var g: Dictionary
var gr: Dictionary


func _init(id: int, _in: int, _out:int, w: float, _recurent := false, _enabled := true) -> void:
	innovation_id = id
	in_id = _in
	out_id = _out
	weight = w
	recurent = _recurent
	enabled = _enabled


func make_connection(from: int, to: int) -> void:
	if not from in g:
		g[from] = []
	if not to in gr:
		gr[to] = []
	g[from].append(to)
	gr[to].append(from)
