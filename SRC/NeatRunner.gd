extends Node

export var data := [[[0,0],[0]], [[1,0],[1]], [[0,1],[1]], [[1,1],[0]]]

var population: NEATPopulation

func _init() -> void:
	randomize()
	population = NEATPopulation.new(1, 2, 1, 1, true)


func _ready() -> void:
	get_parent().network = population.networks[0]


func run_population() -> void:
	for network in population.networks:
		for set in data:
			var output: Array = network.run_inputs(set[0])
			var fitness := 0.0
			for i in range(len(output)):
				fitness += 1-abs(set[1][i]-output[i])
			network.fitness = fitness
