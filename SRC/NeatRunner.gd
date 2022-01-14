extends Node

export var data := [[[0,0],[0]], [[1,0],[1]], [[0,1],[1]], [[1,1],[0]]]

var population: NEATPopulation

func _init() -> void:
	randomize()
	population = NEATPopulation.new(1, 2, 1, 1, true)


func _ready() -> void:
	population.networks[0].mutation_make_node()
	population.networks[0].mutation_make_connection()
	get_parent().network = population.networks[0]


func run_population() -> void:
	for network in population.networks:
		for set in data:
			var output: Array = network.run_inputs(set[0])
			var fitness := 0.0
			for i in range(len(output)):
				fitness += 1-abs(set[1][i]-output[i])
			network.fitness = fitness


func _on_Button_pressed() -> void:
	population.networks[0].mutation_make_connection()
	get_parent().update()


func _on_Button2_pressed() -> void:
	population.networks[0].mutation_make_node()
	get_parent().update()


func _on_Button3_pressed() -> void:
	population.networks[0].mutation_disable_connection()
	get_parent().update()
