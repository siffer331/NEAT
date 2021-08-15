class_name NEATPopulation

var connection_ids := {}
var networks := []
var innovation_number := 1
var nodes := 0


func _init(
	population_size: int,
	in_count: int,
	out_count: int,
	hidden_count: int,
	bias := false
) -> void:
	for i in range(population_size):
		networks.append(NEATNetwork.new(self, in_count, out_count, hidden_count, bias))


