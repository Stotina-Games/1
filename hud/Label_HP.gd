extends Label

const Heart = "♥️";
const EmptyHeart = "♡";
const MaxHearts: int = 20;

func _process(_delta):
	var maxHearts: int = min(MaxHearts, gameplay.Instance.PlayerMaxHP);
	var hpPercent: float = float(gameplay.Instance.PlayerHP) / gameplay.Instance.PlayerMaxHP;
	var fullHearts = floor((maxHearts * hpPercent) + 0.5);

	var sb = ["HP: "];
	for i in range(fullHearts):
		sb.push_back(Heart);
	for i in range(maxHearts - fullHearts):
		sb.push_back(EmptyHeart);

	sb.push_back("  [");
	sb.push_back(gameplay.Instance.PlayerHP);
	sb.push_back("/");
	sb.push_back(gameplay.Instance.PlayerMaxHP);
	sb.push_back("]");


	sb.push_back("  [");
	sb.push_back((int)(hpPercent * 100));
	sb.push_back("%]");

	sb.push_back("  [");
	sb.push_back(fullHearts);
	sb.push_back(Heart);
	sb.push_back("/");
	sb.push_back(maxHearts);
	sb.push_back(EmptyHeart);
	sb.push_back("]");

	text = "".join(sb);
