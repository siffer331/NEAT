class_name NEAT_Visuals
extends Node2D

export var size := 20
export(Array, Color) var node_colors := [Color.blue, Color.red, Color.purple, Color.green]
export(Array, Color) var line_colors := [Color.red, Color.green, Color.blue, Color.purple]

var network: NEATNetwork setget _set_network


func _set_network(value: NEATNetwork) -> void:
	network = value
	update()


func _draw() -> void:
	if network:
		var layers := {}
		var layer_count := []
		var que = []
		var layer = -1
		for node in network.nodes:
			if node.type in [NEATNode.TYPES.INPUT, NEATNode.TYPES.BIAS]:
				que.append(node.id)
		while len(que) > 0:
			layer += 1
			layer_count.append(0)
			var next := []
			for id in que:
				layers[id] = layer
				for connection in network.connections:
					if connection.in_id == id and not connection.recurent:
						next.append(connection.out_id)
			que = next
		var y := {}
		for node in network.nodes:
			y[node.id] = layer_count[layers[node.id]]
			layer_count[layers[node.id]] += 1
		for connection in network.connections:
			var i = layers[connection.in_id]
			var j = y[connection.in_id]
			var p1 := Vector2(i*2.0/layer-1, 0)
			if layer_count[i] == 1:
				p1.y = 0
			else:
				p1.y = (j)*2.0/(layer_count[i]-1)-1
			i = layers[connection.out_id]
			j = y[connection.out_id]
			var p2 := Vector2(i*2.0/layer-1, 0)
			if layer_count[i] == 1:
				p2.y = 0
			else:
				p2.y = (j)*2.0/(layer_count[i]-1)-1
			draw_line(
				p1*size,
				p2*size,
				line_colors[int(connection.enabled)+2*int(connection.recurent)],
				0.02*size
			)
		for node in network.nodes:
			var i = layers[node.id]
			var j = y[node.id]
			var p := Vector2(i*2.0/layer-1, 0)
			if layer_count[i] == 1:
				p.y = 0
			else:
				p.y = (j)*2.0/(layer_count[i]-1)-1
			draw_circle(p*size, 0.1*size, node_colors[node.type])
