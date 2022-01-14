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
		_make_node(NEATNode.TYPES.BIAS)
	for i in range(in_count):
		_make_node(NEATNode.TYPES.INPUT)
	for i in range(out_count):
		_make_node(NEATNode.TYPES.OUTPUT)
	for i in range(hidden_count):
		_make_node(NEATNode.TYPES.HIDDEN)
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
						_make_connection(node1.id, node2.id, randf()*2-1)

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


func mutation_make_node() -> void:
	for try in range(5):
		var connection: NEATConnection = connections[randi()%len(connections)] 
		if connection.enabled:
			connection.enabled = false
			var id := _make_node(NEATNode.TYPES.HIDDEN)
			_make_connection(connection.in_id, id, randf()*2-1)
			_make_connection(id, connection.out_id, randf()*2-1)
			break


func mutation_disable_connection() -> void:
	for try in range(5):
		var connection = connections[randi()%len(connections)]
		if connection.enabled:
			connection.enabled = false
			break


func mutation_make_connection() -> void:
	for try in range(5):
		var a: int = randi()%population.nodes
		var b: int = randi()%population.nodes
		if a == b:
			b = (b+1)%population.nodes
		if population.recurent_prob > randf():
			if population.is_recurent(b, a):
				var c = a
				a = b
				b = c
			if population.is_recurent(a, b):
				var con := _find_connection(a, b)
				if con and con.enabled:
					con.enabled = true
					break
				elif not con:
					_make_connection(a, b, randf()*2-1, true)
					break
		else:
			if not population.is_recurent(b, a):
				var c = a
				a = b
				b = c
			if (
				node_ids[b].type == NEATNode.TYPES.INPUT or
				node_ids[b].type == NEATNode.TYPES.BIAS
			):
				continue
			if not population.is_recurent(a, b):
				var con := _find_connection(a, b)
				if con and con.enabled:
					con.enabled = true
					break
				elif not con:
					print(a, " ", b, " ", node_ids[b].type)
					_make_connection(a, b, randf()*2-1)
					break


func _find_connection(a: int, b: int) -> NEATConnection:
	for connection in connections:
		if connection.in_id == a and connection.out_id == b:
			return connection
	return null


func _make_node(type) -> int:
	var node = NEATNode.new(type, population.nodes)
	population.nodes += 1
	nodes.append(node)
	node_ids[node.id] = node
	return node.id


func _make_connection(in_id: int, out_id: int, weight: float, recurent := false) -> void:
	var innovation: int = population.innovation_number+1
	if in_id in population.connection_ids:
		if out_id in population.connection_ids[in_id]:
			innovation = population.connection_ids[in_id][out_id]
	else:
		population.connection_ids[in_id] = {}
	population.connection_ids[in_id][out_id] = innovation
	if not recurent:
		population.make_connection(in_id, out_id)
	connections.append(NEATConnection.new(innovation, in_id, out_id, weight, recurent))
	


