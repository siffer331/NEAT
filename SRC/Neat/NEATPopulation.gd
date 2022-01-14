class_name NEATPopulation

var recurent_prob := 0.0

var connection_ids := {}
var networks := []
var innovation_number := 1
var nodes := 0
var g := {}


func _init(
	population_size: int,
	in_count: int,
	out_count: int,
	hidden_count: int,
	bias := false
) -> void:
	for i in range(population_size):
		networks.append(NEATNetwork.new(self, in_count, out_count, hidden_count, bias))


func make_connection(a: int, b: int) -> void:
	if not a in g:
		g[a] = []
	if not b in g[a]:
		g[a].append(b)


func is_recurent(a: int, b: int) -> bool:
	if b in g:
		for to in g[b]:
			if to == a:
				return true
			if is_recurent(a, to):
				return true
	return false


