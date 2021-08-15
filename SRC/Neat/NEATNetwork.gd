class_name NEATNetwork

var nodes := []
var node_ids := {}
var connections := []
var population
var fitness: float
var speciec_id := -1

func _init(
	_population,
	in_count: int,
	out_count: int,
	hidden_count: int,
	bias := false
) -> void:
	population = _population
	if bias:
		_make_node(NEATNode.new(NEATNode.TYPES.BIAS, 0))
	for i in range(in_count):
		_make_node(NEATNode.new(NEATNode.TYPES.INPUT, i+1))
	for i in range(out_count):
		_make_node(NEATNode.new(NEATNode.TYPES.OUTPUT, i+1+in_count))
	for i in range(hidden_count):
		_make_node(NEATNode.new(NEATNode.TYPES.HIDDEN, i+1+in_count+out_count))
	if hidden_count > 0:
		for node1 in nodes:
			if node1.type in [NEATNode.TYPES.INPUT, NEATNode.TYPES.BIAS]:
				for node2 in nodes:
					if node2.type == NEATNode.TYPES.HIDDEN:
						_make_connection(node1.id, node2.id, (randf()*2-1)*randf())
		for node1 in nodes:
			if node1.type == NEATNode.TYPES.HIDDEN:
				for node2 in nodes:
					if node2.type == NEATNode.TYPES.OUTPUT:
						_make_connection(node1.id, node2.id, (randf()*2-1)*randf())
	else:
		for node1 in nodes:
			if node1.type in [NEATNode.TYPES.INPUT, NEATNode.TYPES.BIAS]:
				for node2 in nodes:
					if node2.type == NEATNode.TYPES.OUTPUT:
						_make_connection(node1.id, node2.id, (randf()*2-1)*randf())

func run_inputs(inputs: Array) -> Array:
	for node in nodes:
		node.old_output = node.output
		node.input = 0
		node.calculated = false
	var index = 0
	for node in nodes:
		if node.type == NEATNode.TYPES.BIAS:
			node.input = 1
		elif node.type == NEATNode.TYPES.INPUT:
			node.input = inputs[index]
			index += 1
			node.calculated = true
	var outputs := []
	for node in nodes:
		if node.type == NEATNode.TYPES.OUTPUT:
			solve(node.id)
			outputs.append(node.output)
	return outputs


func solve(id: int) -> void:
	var node: NEATNode = node_ids[id]
	if node.calculated:
		return
	for connection in connections:
		if connection.out_id == id:
			var from: NEATNode = node_ids[connection.in_id]
			if connection.recurent:
				node.input += from.old_output
			else:
				solve(from.id)
				node.input += from.output*connection.weight
	node.output = 1/(1+exp(node.input))
	node.calculated = true


func _make_node(node: NEATNode) -> void:
	nodes.append(node)
	node_ids[node.id] = node


func _make_connection(in_id: int, out_id: int, weight: float) -> void:
	var innovation: int = population.innovation_number+1
	if in_id in population.connection_ids:
		if out_id in population.connection_ids[in_id]:
			innovation = population.connection_ids[in_id][out_id]
	else:
		population.connection_ids[in_id] = {}
	population.connection_ids[in_id][out_id] = innovation
	connections.append(NEATConnection.new(innovation, in_id, out_id, weight))
	


