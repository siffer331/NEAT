class_name NEATNode

enum TYPES {INPUT, OUTPUT, HIDDEN, BIAS}

var id: int
var type: int

var input: float
var old_output: float
var output: float
var calculated := false



func _init(_type: int, _id: int) -> void:
	id = _id
	type = _type
