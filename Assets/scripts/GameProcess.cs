using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;
#nullable enable
public class Hook
{
    private string argName;
    private string dependentArgName;
    public float num;
    private Roles cls;
    private string op;
    public Hook(string argName, string dependentArgName, float num, string op, Roles cls)
    {
        this.argName = argName;
        this.dependentArgName = dependentArgName;
        this.num = num;
        this.op = op;
        this.cls = cls;
        cls.AddHook(this);
    }

    public string ArgName { get { return argName; } }
    public string DependentArgName { get { return dependentArgName; } }
    
    public string Op { get { return op; } }

    public float Num { get {return this.num;}}
}

public class Roles
{
    public bool again = false;
    public bool all_fast_card = false;
    public Dictionary<string, float> temphook = new();
    private Dictionary<string, float> properties = new();
    public List<Cards> card_pack_instance = new();
    public List<string> track_list = new();
    public List<string> only_list = new();
    public List<string> accumulateList = new();
    public string role_name = "";
    public bool respone_complete;
    public int role_index;
    public bool harm_to_life_next = false;
    public GameProcess process = new();
    public List<Cards> card_pack_instance_backup = new();
    public static int life_init = 1000000;
    public static Dictionary<string, string> name_args = new()
    {
        { "shield", "护盾" },
        { "power", "力量" },
        { "mana", "法力" },
        { "coin", "幸运币" },
        { "heal", "自愈" },
        { "bleed", "流血" },
        { "weak", "虚弱" },
        { "easy_hurt", "易伤" },
        { "note", "乐符" }
    };  
    public Roles() {}
    
    public Roles(string role_name, List<string> card_pack, GameProcess process)
    {
        InitializeProperty("coin_", 0);
        // 在构造函数中初始化属性
        InitializeProperty("life_max", life_init);
        InitializeProperty("life_now", life_init);
        InitializeProperty("life_recover", 0);
        InitializeProperty("shield", 0);
        InitializeProperty("power", 0);
        InitializeProperty("coin", 0);
        InitializeProperty("bleed", 0);
        InitializeProperty("heal", 0);
        InitializeProperty("note", 0);
        InitializeProperty("mana", 0);
        InitializeProperty("rampart", 0);
        InitializeProperty("weak", 0);
        InitializeProperty("easy_hurt", 0);
        InitializeProperty("coin_judge_min", 1);
        InitializeProperty("attack", 0);
        InitializeProperty("effect_count_even", 1);
        InitializeProperty("effect_count_next", 1);
        InitializeProperty("coin_throw_beilv", 1);
        InitializeProperty("card_odd", 0);
        InitializeProperty("card_even", 0);
        InitializeProperty("card_accumulate", 0);
        InitializeProperty("card_use_count", 0);
        InitializeProperty("attack_count", 0);
        InitializeProperty("card_use_index", 0);
        InitializeProperty("fast_card_limit", 0);
        InitializeProperty("harm_to_life", 0);
        InitializeProperty("turn_count", 0);
        InitializeProperty("harm", 0);
        InitializeProperty("heal_beilv", 1);
        InitializeProperty("bleed_harm", 0);
        InitializeProperty("bleed_to_life", 0);
        InitializeProperty("bleed_count", 0);
        InitializeProperty("recover_count", 0);
        InitializeProperty("life_change", 0);
        InitializeProperty("push_bleed", 0);
        InitializeProperty("const_num", 1);
        InitializeProperty("attack_note", 0);
        this.role_name = role_name;
        this.process = process;
        process.role_list.Add(this);
        if (process.role_list.Count == 1)
        {
            this.role_index = 0;
        }
        else
        {
            this.role_index = 1;
        }

        // 卡组
        int order = 0;
        foreach (var card in card_pack)
        {
            var temp = card.Split("_");
            var card_name = temp[0];
            var card_level = int.Parse(temp[1]);
            card_pack_instance.Add(new Cards(this, card_name, order, card_level));
            this.card_pack_instance_backup.Add(new Cards(this, card_name, order, card_level));
            order++;
        }
    }

    private List<Hook> hooks = new();
    public List<string> onlyList = new();
    public bool no_limit = false;

    public string role_describe = "";
    public string log = "";

    public bool return_note = false;

    public void TurnBegin() {
        var enemy = this.process.role_list[(this.role_index + 1) % 2];
        if (this["rampart"] <= 1 && this["turn_count"] != 1)
        {
            this["shield"] = this["shield"] / 2;
        }
        enemy["bleed_harm"] += this["bleed"] * 30;
        enemy["bleed_count"] += 1;
        this["life_recover"] += this["heal"];
        this["turn_count"] += 1;
        MergeAccumulate();
        AccumulateAccelerate();
    }
    public void turnEnd() {
        var enemy = this.process.role_list[(this.role_index + 1) % 2];
        this["weak"] = Math.Max(0, this["weak"] - 1);
        this["easy_hurt"] = Math.Max(0, this["easy_hurt"] - 1);
        enemy["bleed"] += this["push_bleed"];
        this["push_bleed"] = 0;
        enemy["push_bleed"] = 0;
    }
    public void RoleLoad()
    {
        
        if (this.role_name == "西琳")
        {
            this.role_describe = "西琳:开始获得2层力量,10护盾,使用奇数牌增加1点护盾,偶数牌增加1点生命";
            this["shield"] += 300;
            this["power"] += 2;
            new Hook("card_odd", "shield", 30, "+", this);
            new Hook("card_even", "life_recover", 1, "+", this);        

        }
        if (this.role_name == "特丽丽")
        {
            this.role_describe = "特丽丽:开始获得一个幸运币,判定不小于3,每获得一个幸运币,回复1生命";
            this["coin"] += 1;
            this["coin_judge_min"] = 3;
            new Hook("coin", "life_recover", 1, "+", this);
        }

        if (this.role_name == "芙乐艾")
        {
            this.role_describe = "芙乐艾:开始获得2层法力并为敌方施加1层虚弱,2层流血";
            this.process.role_list[(int)(this.role_index + 1) % 2]["weak"] += 1;
            this.process.role_list[(int)(this.role_index + 1) % 2]["bleed"] += 2;
            this["mana"] += 2;
        }
        if (this.role_name == "布洛洛")
        {
            this.role_describe = "布洛洛:开始获得2层自愈,每消耗1层自愈,增长3生命,生命变化时,增加1护盾";
            new Hook("heal", "life_recover", 3, "-", this);
            new Hook("life_change", "shield", 30, "+", this);
            this["heal"] += 2;
        }
        if (this.role_name == "绮罗老师")
        {
            this.role_describe = "绮罗老师:开始获得1乐符,4法力,每获得8枚乐符,获得1层力量";
            this["note"] += 1;
            this["mana"] += 4;
            new Hook("note", "power", 0.125f, "+", this);
        }
        if (this.role_name == "学园长")
        {
            this.role_describe = "学园长:开始获得1法力,5点护盾,15点生命上限,每次释放积蓄卡牌,获得3护盾";
            new Hook("card_accumulate", "shield", 90, "+", this);
            this["mana"] += 1;
            this["shield"] += 5 * 30;
            this["life_now"] += 15 * 30;
            this["life_max"] += 15 * 30;
        }
    }
    public int UseCard()
    {
        int tempCount = 0;
        var card_use = this.card_pack_instance[(int)this["card_use_index"]];
        if (this["mana"] >= card_use.mana)
        {
            tempCount += 1;
            this["mana"] -= card_use.mana;
            this["card_use_count"]++;
            float effect_count_next = this["effect_count_next"];
            for (int i = 0; i < effect_count_next; i++)
            {
                this["effect_count_next"] = 1;
                card_use.Use();
            }
            

            if (card_use.odd && (card_use.index % 2 == 0 || this.no_limit))
            {
                this["card_odd"]++;
            }
            if (card_use.even && (card_use.index % 2 == 1 || this.no_limit))
            {
                this["card_even"]++;
                for (int i = 0; i < this["effect_count_even"] - 1; i++)
                {
                    card_use.Use();
                }
            }

            if (card_use.broken)
            {
                this.card_pack_instance.Remove(card_use);
                this["card_use_index"] = this["card_use_index"] % this.card_pack_instance.Count;
            }
            else
            {
                this["card_use_index"] = (this["card_use_index"] + 1) % this.card_pack_instance.Count;
            }

            if ((card_use.fast_card || this.all_fast_card) && this["fast_card_limit"] == 0)
            {
                this["fast_card_limit"] = (this["fast_card_limit"] + 1) % 2;
                int result = UseCard();
                this["fast_card_limit"] = (this["fast_card_limit"] + 1) % 2;
                return result + tempCount;
            }else {
                return tempCount;
            }
        }
        else
        {
            this.log += "\n法力不足, 法力加1\n";
            this["mana"] += 1;
            return 0;
        }
    }


    

    public void InitializeProperty(string propName, int value)
    {
        properties[propName] = value;
        temphook[propName] = value;
    }

    public float this[string propName]
    {
        get
        {
            if (properties.ContainsKey(propName))
            {
                return properties[propName];
            }
            else
            {
                return 0;
            }
        }
        set
        {
            float result = ApplyHooks(propName, value);
            if (result == 0) {
                properties[propName] = value;
            }else {
                properties[propName] = value;
                this[propName] += result;
            }
            if(properties.ContainsKey(propName + "_")) {
                if(properties[propName + "_"] != 0) {
                    temphook[propName + "_"] += 1;
                    if (temphook[propName + "_"] % 2 == 1) {
                        this[propName] += properties[propName + "_"];
                        properties[propName + "_"] = 0;
                    }

                }                
            }
        }
    }

    public void AddHook(Hook hook)
    {
        foreach (var hook_ in hooks) {
            if (hook_.ArgName == hook.ArgName && hook_.DependentArgName == hook.DependentArgName &&
                hook_.Op == hook.Op) {
                    hook_.num += hook.num;
                    return;
            }
        }
        hooks.Add(hook);
    }

    private float ApplyHooks(string propName, float value)
    {
        foreach (var hook in hooks)
        {
            if (propName == hook.ArgName)
            {
                if (properties.ContainsKey(propName))
                {
                    float delta = value - properties[propName];
                    if (hook.Op == "+" && delta > 0)
                    {
                            this[hook.DependentArgName] += hook.Num * delta;
                    }
                    else if (hook.Op == "-" && delta < 0)
                    {
                        if(hook.DependentArgName == hook.ArgName) {

                            return (-hook.Num) *  delta;
                        }else {
                            this[hook.DependentArgName] -= hook.Num * delta;
                            return 0;
                        }
                    }
                }
                
            }
        }
        return 0;
    }

    public int throw_coin(float num) {
        num = (this["coin"] >= num) ? num : this["coin"];
        this["coin"] -= num;
        int attack = 0;
        System.Random random = new();

        for (int i = 0; i < (num * this["coin_throw_beilv"]); i++)
        {
            attack += random.Next((int)this["coin_judge_min"], 7); // 7 to include 6
        }

        return attack;

    }
    public void AccumulateAccelerate(int accelerateNum = 1, int effectNum = 1, bool noLimit = false, int min = 0) {
        //max > now > attr_name_change > attr_name_accord > beilv > color > accu_num
        List<string> removeList = new List<string>();
        for (int idx = 0; idx < accumulateList.Count; idx++)
        {
            string i = accumulateList[idx];
            string[] temp = i.Replace(" ", "").Split(">");
            int countMax = int.Parse(temp[0]);
            string attrName = temp[2];
            string attrNameAccord = temp[3];
            float beilv = float.Parse(temp[4]);
            int num = (int)((int)this[attrNameAccord] * beilv);
            int countNow = int.Parse(temp[1]);
            string color = temp[5];
            int accuNum = int.Parse(temp[6]);
            if (min != 0)
            {
                countNow = min;
            }
            else
            {
                countNow -= (countNow < accelerateNum) ? countNow : accelerateNum;
            }

            if (countNow <= 0 || noLimit)
            {
                this[attrName] += num * accuNum * effectNum;
                if (!noLimit)
                {
                    if (!again)
                    {
                        removeList.Add(i);
                    }
                    else
                    {
                        string strNew = $"{countMax} > {countMax} > {attrName} > {attrNameAccord} > {beilv} > {color} > {accuNum}";
                        accumulateList[idx] = strNew;
                    }
                }
            }
            else
            {
                string strNew = $"{countMax} > {countNow} > {attrName} > {attrNameAccord} > {beilv} > {color} > {accuNum}";
                accumulateList[idx] = strNew;
            }
        }

        foreach (var item in removeList)
        {
            accumulateList.Remove(item);
        }
    }
    public int GetAccumulateNum()
    {
        int num = 0;
        List<string> accumulateListCopy = new List<string>(accumulateList);

        for (int idx = 0; idx < accumulateListCopy.Count; idx++)
        {
            string i = accumulateListCopy[idx];
            string[] temp = i.Replace(" ", "").Split('>');

            if (temp.Length > 4)
            {
                string color = temp[5];
                if (color != "none")
                {
                    num += int.Parse(temp[6]);
                }
            }
        }

        return num;
    }    
    public int GetAccumulateMin()
    {
        List<int> numList = new List<int>();

        foreach (string i in accumulateList.ToList())
        {
            string[] temp = i.Replace(" ", "").Split('>');

            if (temp.Length > 3 && int.TryParse(temp[1], out int countNow))
            {
                numList.Add(countNow);
            }
        }

        if (numList.Count > 0)
        {
            return numList.Min();
        }

        // Return a default value if the list is empty
        return 0;
    }    
    public void MergeAccumulate() {
        accumulateList = MergeStrings(accumulateList);
    }
    public List<string> MergeStrings(List<string> strings)
    {
        var mergedDict = new Dictionary<string, float>();

        foreach (var str in strings)
        {
            string[] parts;
            string key;
            float value;
            if (str.Contains(">"))
            {
                parts = str.Split(new string[] { " > " }, StringSplitOptions.None);
                key = string.Join(" > ", parts, 0, parts.Length - 1);
                value = float.Parse(parts[^1], CultureInfo.InvariantCulture); // Parse as float
                if (!mergedDict.TryAdd(key, value))
                {
                    mergedDict[key] += value;
                }
            }
        }

        var mergedStrings = new List<string>();
        foreach (var kvp in mergedDict)
        {
            if (kvp.Key.Contains(">"))
            {
                mergedStrings.Add($"{kvp.Key} > {kvp.Value}");
            }
        }

        return mergedStrings;
    }
}



public class Cards
{
    public int index = 0;
    public bool fast_card = false;
    public bool broken = false;
    public bool odd = false;
    public bool even = false;
    public int mana = 0;
    public string? color;
    public string? describe;
    public string card_name;
    public int level;
    public Roles role;
    private Action? use;
    public void Use()
    {

        // Call the bound "use" function
        use?.Invoke();
    }
    public Cards(Roles role, string card_name, int index, int level) {
        this.describe = "";
        this.card_name = card_name;
        this.index = index;
        this.level = level;
        this.role = role;
        string color = "红";
        if (card_name == "垒之护") {
            this.color = color;
            this.describe = $"护盾+{25 + 25 * level},获得99层壁垒";
            Action use = () =>
            {
                role["rampart"] += 99;
                role["shield"] += (25 + 25 * level) * 30;
            };
            this.use = use;            
        }
        if (card_name == "绚烂.星霞") {
            this.color = color;
            this.describe = $"现有力量翻{1 + level}倍";
            Action use = () => {
                role["power"] *= 1 + level;
            };
            this.use = use;  
        }
        if (card_name == "跃增.运时") {
            this.color = color;
            this.describe = $"幸运币+{10 * level},魔阵:消耗等量幸运币后返还等量幸运币,唯一";
            this.broken = true;
            Action use = () => {
                role["coin"] += 10 * level;
                if (!role.onlyList.Contains(card_name))
                {
                    role.onlyList.Add(card_name);
                    new Hook("coin", "coin", 1, "-", role);
                }
                
            };
            this.use = use;  
        } 
        color = "金";
        if (card_name == "矛之突") {
            this.color = color;
            this.describe = $"护盾加{5 + 5 * level},然后失去所有护盾,每失去一点造成{2 + level}伤害";
            Action use = () => {
                role["shield"] += (5 + 5 * level) * 30;
                float shield_num = role["shield"];
                role["harm"] += shield_num * (2 + level);
                role["shield"] = 0;
                role["attack_count"] += 1;
            };
            this.use = use;  
        } 
        if (card_name == "灼灼.星熠") {
            this.color = color;
            int power_add = (level == 1) ? 2 : 4;
            int even_count = (level == 3) ? 3 : 2;
            this.describe = $"魔阵:打出奇数牌获得{power_add}力量,偶数牌重复{even_count}次效果,唯一";
            this.broken = true;
            Action use = () => {
                if (!role.onlyList.Contains(card_name))
                {
                    new Hook("card_odd", "power", power_add, "+", role);
                    role["effect_count_even"] = even_count;
                    role.onlyList.Add(card_name);
                }
            };
            this.use = use;  
        }
        if (card_name == "复加.得时") {
            this.color = color;
            this.describe = $"魔阵:投掷的幸运币数量翻{1 + level}倍,唯一";
            this.broken = true;
            Action use = () => {
                role["coin_throw_beilv"] = 1 + level;
            };
            this.use = use;  
        }  
        if (card_name == "幸运一掷") {
            this.color = color;
            this.describe = $"{10 + level * 5}伤害,投掷所有幸运币,造成总点数{Math.Round(0.7 + level * 0.3)}的伤害";

            Action use = () => {
                role["attack"] += 10 + level * 5;
                int result = role.throw_coin(role["coin"]);
                role["attack"] += result;
                role["attack_count"] += 1;
            };
            this.use = use;  
        }
        if (card_name == "时来运转") {
            this.color = color;
            int num = (level == 1) ? 10 : 6 + level * 3;
            this.describe = $"幸运币+{num}";
            
            Action use = () => {
                int num = (level == 1) ? 10 : 6 + level * 3;
                role["coin"] += num;
            };
            this.use = use;  
        }
        if (card_name == "好运加护") {
            this.color = color;
            this.broken = true;
            this.describe = $"幸运币+{1 + level * 2},魔阵:幸运币必投出最大值";
            Action use = () => {
                int num = 1 + level * 2;
                role["coin"] += num;
                role["coin_judge_min"] = 6;
                
            };
            this.use = use;  
        }
        if (card_name == "魔阵.星数") {
            this.color = color;
            this.broken = true;
            if (level == 2)
            {
                this.describe = "获得1层力量,奇数-魔阵:每回合获得一层力量";
            }
            else
            {
                this.describe = $"奇数-魔阵:每回合获得{(level == 3 ? 2 : 1)}层力量";
            }

            Action use = () => {
                if (level == 2)
                {
                    role["power"] += 1;
                }

                if (this.index % 2 == 0 || role.no_limit)
                {
                    int num = (level == 3) ? 2 : 1;
                    new Hook("turn_count", "power", num, "+", role);
                }
            };
            this.use = use;  
        }
        if (card_name == "焰力.星数") {
            this.color = color;
            this.even = true;
            this.describe = $"{level + 3}伤害,偶数:每用过1张牌,追加{level + 3}伤害";
            Action use = () => {
                int num = 3 + level;
                role["attack"] += num;
                role["attack_count"] += 1;

                if (this.index % 2 == 1 || role.no_limit)
                {
                    role["attack"] += num * role["card_use_count"];
                }
            };
            this.use = use;  
        }
        if (card_name == "幻记.星数") {
            this.color = color;
            this.broken = true;
            this.describe = $"获得{2 + level}层力量,魔阵:奇数偶数无条件触发";
            Action use = () => {
                role["power"] += 2 + level;
                role.no_limit = true;
            };
            this.use = use;  
        }
        color = "紫";

        if (card_name == "盾之守") {
            this.color = color;
            this.describe = $"护盾+{5 + level},然后护盾翻倍";
            Action use = () => {
                role["shield"] += (5 + level) * 30;
                role["shield"] *= 2;
            };
            this.use = use;  
        }
        if (card_name == "烁烁.星电") {
            this.color = color;
            this.describe = $"奇数:施加{3 + level}层易伤,偶数:施加{3 + level}层虚弱";

            if (this.index % 2 == 0)
            {
                this.even = true;
            }
            else
            {
                this.odd = true;
            }
            Action use = () => {
                var enemy = role.process.role_list[(role.role_index + 1) % 2];
                if (this.index % 2 == 0)
                {
                    enemy["easy_hurt"] += 3 + level;
                }
                else
                {
                    enemy["weak"] += 3 + level;
                }
            };
            this.use = use;  
        }
        if (card_name == "倍加.巧时") {
            this.color = color;
            this.describe = $"幸运币+{-5 + level * 5},魔阵:获得的幸运币数量翻倍,唯一";
            this.broken = true;
            Action use = () => {
                role["coin"] += -5 + level * 5;
                role["coin_get_beilv"] = 2;
                if (!role.onlyList.Contains(card_name))
                {
                    role.onlyList.Add(card_name);
                    new Hook("coin", "coin_", 1, "+", role);

                }                
            };
            this.use = use;  
        }
        if (card_name == "术士.魔阵") {
            this.color = color;
            this.broken = true;
            this.describe = $"魔阵:每回合幸运币+{level}";            
            Action use = () => {
                new Hook("turn_count", "coin", level, "+", role);
            };
            this.use = use;  
        }
        if (card_name == "骑士.魔运") {
            this.color = color;
            this.describe = $"生效{4 + level * 2}次:2+点数伤害";
            Action use = () => {
                int num = 4 + level * 2;
                role["attack"] += 2 * num;
                int result = role.throw_coin(num);
                role["attack"] += result;
                role["attack_count"] += num;

            };
            this.use = use;  
        }
        if (card_name == "医者.魔运") {
            this.color = color;
            this.describe = $"幸运币+{(level == 1 ? 4 : 6)}, 每有一个幸运币加{(level == 3 ? 2 : 1)}生命";
            Action use = () => {
                role["coin"] += level == 1 ? 4 : 6;
                role["life_recover"] += (level == 3 ? 2 : 1) * role["coin"];
            };
            this.use = use;  
        }
        if (card_name == "战车.魔运") {
            this.color = color;
            this.describe = $"魔阵:每获得一个幸运币造成{level}伤害";
            this.broken = true;
            Action use = () => {
                new Hook("coin", "attack", level, "+", role);
            };
            this.use = use;  
        }
        if (card_name == "伤.星数") {
            this.color = color;
            this.describe = $"5伤害,奇数:施加{2 + level}层易伤";
            this.odd = true;
            this.mana = 1;
            Action use = () => {
                var enemy = role.process.role_list[(role.role_index + 1) % 2];
                role["attack"] += 5;
                role["attack_count"] += 1;
                if (this.index % 2 == 0 || role.no_limit)
                {
                    enemy["easy_hurt"] += 2 + level;
                }                
            };
            this.use = use;  
        }
        if (card_name == "灵.星数") {
            this.color = color;
            this.describe = $"2伤害*{2 + level},偶数:法力加{level}";
            this.even = true;
            Action use = () => {
                int num = 2 + level;
                role["attack"] += 2 * num;
                role["attack_count"] += num;
                if (this.index % 2 == 1 || role.no_limit)
                {
                    role["mana"] += level;
                }
            };
            this.use = use;  
        }
        if (card_name == "赐.星数") {
            this.color = color;
            this.describe = $"2伤害*{1 + level},偶数:每用过一张牌,生命加{2 + level}";
            this.even = true;
            Action use = () => {
                role["attack"] += 2 * (1 + level);
                role["attack_count"] += (1 + level);
                if (this.index % 2 == 1 || role.no_limit)
                {
                    role["life_recover"] += role["card_use_count"] * (2 + level);
                }
            };
            this.use = use;  
        }
        if (card_name == "势.星数") {
            this.color = color;
            this.describe = $"获得2层力量,奇数:再获得{level}层力量";
            this.odd = true;
            Action use = () => {
                role["power"] += 2;
                if (this.index % 2 == 0 || role.no_limit)
                {
                    role["power"] += level;
                }
            };
            this.use = use;  
        }
        color = "红";
        if (card_name == "光羽夜蝶") {
            this.color = color;
            this.describe = $"魔阵:每回合获得{level}层力量,自愈与法力,敌方获得{level}层流血,虚弱和易伤";
            this.broken = true;
            Action use = () => {
                var enemy = role.process.role_list[(role.role_index + 1) % 2];
                new Hook("turn_count", "power", level, "+", role);
                new Hook("turn_count", "heal", level, "+", role);
                new Hook("turn_count", "mana", level, "+", role);
                new Hook("turn_count", "bleed", level, "+", enemy);
                new Hook("turn_count", "weak", level, "+", enemy);
                new Hook("turn_count", "easy_hurt", level, "+", enemy);
            };
            this.use = use;  
        }
        if (card_name == "收割.夜蝶") {
            this.color = color;
            this.describe = $"{20 + level * 10}伤害,追加等于敌方已损失生命值{level}倍的流血伤害";
            Action use = () => {
                var enemy = role.process.role_list[(role.role_index + 1) % 2];
                role["attack_count"] += 1;
                role["attack"] += 20 + level * 10;
                role["bleed_harm"] += (int)(enemy["life_max"] - enemy["life_now"]) * level;
                role["bleed_count"] += 1;
            };
            this.use = use;  
        }
        if (card_name == "复原.光羽") {
            this.color = color;
            this.describe = $"下{2 * (level == 3 ? 2 : 1)}回合自身受到的所有伤害转为治疗";
            if (level == 1)
            {
                this.mana= 2;
            }
            Action use = () => {
                if (role["harm_to_life"] == 0)
                {
                    role.harm_to_life_next = true;
                }
                role["harm_to_life"] += 2 * (level == 3 ? 2 : 1);
            };
            this.use = use;  
        }
        color = "金";
        if (card_name == "夜影光烁") {
            this.color = color;
            this.describe = $"{10 * level + 15 + (level == 3 ? 5 : 0)}伤害和生命";
            Action use = () => {
                role["attack"] += 10 * level + 15 + (level == 3 ? 5 : 0);
                role["life_recover"] += 10 * level + 15 + (level == 3 ? 5 : 0);
                role["attack_count"] += 1;
            };
            this.use = use;  
        }
        if (card_name == "血疗.夜蝶") {
            this.color = color;
            this.describe = $"魔阵:自身每有一层流血,回合开始时回复{1 + level}点生命";
            this.broken = true;
            Action use = () => {
                role["bleed_to_life"] += 1 + level;
            };
            this.use = use;  
        }
        if (card_name == "神愈.光羽") {
            this.color = color;
            this.describe = $"魔阵:治疗量变为{level + 1}倍,唯一";
            this.broken = true;
            Action use = () => {
                role["heal_beilv"] = level + 1;
            };
            this.use = use;  
        }
        if (card_name == "渴血.夜蝶") {
            this.color = color;
            this.describe = $"流血对敌方造成伤害时,回复伤害值{10 * level + 15 + (level == 3 ? 5 : 0)}%的生命";
            this.broken = true;
            Action use = () => {
                new Hook("bleed_harm", "life_recover", (10 * level + 15 + (level == 3 ? 5 : 0)) / 100 / 30, "+", role);
                
            };
            this.use = use;  
        }
        if (card_name == "血噬.夜蝶") {
            this.color = color;
            this.describe = $"敌方流血立刻生效{1 + level}次";
            this.mana = 1;
            Action use = () => {
                for (int i = 0; i < 1 + level; i++)
                {
                    role["bleed_harm"] += role["bleed"] * 30;
                }
                role["bleed_count"] += 1 + level;
            };
            this.use = use;  
        }
        if (card_name == "晨昏") {
            this.color = color;
            this.describe = $"双方叠加{2 + level}层流血和自愈 迅捷";
            this.fast_card = true;

            Action use = () => {
                var enemy = role.process.role_list[(role.role_index + 1) % 2];
                role["bleed"] += 2 + level;
                role["heal"] += 2 + level;
                enemy["bleed"] += 2 + level;
                enemy["heal"] += 2 + level;
            };
            this.use = use;  
        }
        if (card_name == "裂伤.夜蝶") {
            this.color = color;
            this.broken = true;
            this.describe = $"敌方当前流血层数+{75 + level * 25}% 消耗";
            this.mana = 2;
            Action use = () => {
                var enemy = role.process.role_list[(role.role_index + 1) % 2];
                enemy["bleed"] += (int)(enemy["bleed"] * (75 + level * 25) / 100);
            };
            this.use = use;  
        }
        if (card_name == "清心.光羽") {
            this.color = color;
            this.describe = $"清除自身所有负面状态,每清除一点回复{1 + level}点生命值";
            this.mana = 1;
            Action use = () => {
                int num_ = (int)(role["bleed"] + role["weak"] + role["easy_hurt"]);
                role["bleed"] = 0;
                role["weak"] = 0;
                role["easy_hurt"] = 0;
                role["life_recover"] += num_ * (1 + level);
            };
            this.use = use;  
        }
        if (card_name == "神罚.光羽") {
            this.color = color;
            this.describe = $"{level}层自愈 魔阵:治疗时对敌方造成等量伤害";
            this.broken = true;
            Action use = () => {
                role["heal"] += level;
                new Hook("life_max", "harm", 1, "+", role);
            };
            this.use = use;  
        }
        if (card_name == "神赐.光羽") {
            this.color = color;
            this.describe = $"{12 + level * 4}伤害 自身每有一层自愈,多{4 + level}伤害";
            this.mana = 1;
            Action use = () => {
                role["attack"] += 12 + level * 4;
                role["attack"] += role["heal"] * (4 + level);
                role["attack_count"] += 1;
            };
            this.use = use;  
        }
        color = "紫";
        if (card_name == "光夜交织") {
            this.color = color;
            this.describe = $"双方叠加{5 + level}层自愈和流血";
            Action use = () => {
                var enemy = role.process.role_list[(role.role_index + 1) % 2];
                role["bleed"] += 5 + level;
                role["heal"] += 5 + level;
                enemy["bleed"] += 5 + level;
                enemy["heal"] += 5 + level;
            };
            this.use = use;  
        }
        if (card_name == "血镰.夜蝶") {
            this.color = color;
            this.describe = $"移除敌方所有自愈,并施加{level}倍层数的流血";
            Action use = () => {
                var enemy = role.process.role_list[(role.role_index + 1) % 2];
                enemy["bleed"] += enemy["heal"] * level;
                enemy["heal"] = 0;
            };
            this.use = use;  
        }
        if (card_name == "静心.光羽") {
            this.color = color;
            this.describe = $"法力加 {0 + (level == 1 ? 0 : 5)} 魔阵:自身负面状态减少时,获得{1 + (level == 3 ? 1 : 0)}倍减少层数的自愈";
            this.broken = true;
            Action use = () => {
                role["mana"] += 0 + (level == 1 ? 0 : 5);
                new Hook("weak", "heal", level == 3 ? 1 : 0, "-", role);
                new Hook("easy_hurt", "heal", level == 3 ? 1 : 0, "-", role);
                new Hook("bleed", "heal", level == 3 ? 1 : 0, "-", role);
            };
            this.use = use;  
        }
        if (card_name == "以血换血.夜蝶") {
            this.color = color;
            this.broken = true;
            this.describe = $"法力+{-2 + 2 * level}魔阵:自身被叠加流血时,向敌方叠加相同层数的流血";

            Action use = () => {
                role["mana"] += -2 + 2 * level;
                new Hook("bleed", "push_bleed", 1, "+", role);
            };
            this.use = use;  
        }
        if (card_name == "灵愈.光羽") {
            this.color = color;
            this.describe = $"法力+{1 + level}, 每有一点法力加{1 + level}点生命";
            Action use = () => {
                role["mana"] += 1 + level;
                role["life_recover"] += (1 + level) * role["mana"];
            };
            this.use = use;  
        }
        if (card_name == "回愈.光羽") {
            this.color = color;
            this.describe = $"生命+10,每有一点自愈多回{1 + level}点生命 迅捷";
            this.mana = 3;
            this.fast_card = true;
            Action use = () => {
                role["life_recover"] += 10;
                role["life_recover"] += (1 + level) * role["heal"];
            };
            this.use = use;  
        }
        if (card_name == "狂宴.夜蝶") {
            this.color = color;
            this.mana = 3;
            this.describe = $"敌方叠加2层流血和{level}层虚弱 迅捷";
            this.fast_card = true;

            Action use = () => {
                var enemy = role.process.role_list[(role.role_index + 1) % 2];
                enemy["bleed"] += 2;
                enemy["weak"] += level;
            };
            this.use = use;  
        }
        if (card_name == "拂晓") {
            this.color = color;
            this.describe = $"法力+{2 + 2 * level} 双方叠加{2 + level}层流血和自愈";
            Action use = () => {
                role["mana"] += 2 + 2 * level;
                var enemy = role.process.role_list[(role.role_index + 1) % 2];
                enemy["bleed"] += 2 + level;
                enemy["heal"] += 2 + level;
                role["bleed"] += 2 + level;
                role["heal"] += 2 + level;
            };
            this.use = use;  
        }
        if (card_name == "刃舞.夜蝶") {
            this.color = color;
            describe = $"法力+{level} 敌方叠加等于自身法力的流血(至多{3 + level * 2})";
            Action use = () => {
                role["mana"] += level;
                var enemy = role.process.role_list[(role.role_index + 1) % 2];
                enemy["bleed"] += (level * 2 + 3) > role["mana"] ? role["mana"] : (level * 2 + 3);

            };
            this.use = use;  
        }
        if (card_name == "血阵.夜蝶") {
            this.color = color;
            describe = $"魔阵:每回合为双方叠加{1 + level}层流血";
            broken = true;
            Action use = () => {
                var enemy = role.process.role_list[(role.role_index + 1) % 2];
                new Hook("turn_count", "bleed", 1 + level, "+", role);
                new Hook("turn_count", "bleed", 1 + level, "+", enemy);
            };
            this.use = use;  
        }
        if (card_name == "血清.夜蝶") {
            this.color = color;
            describe = $"{13 + level * 2}伤害 消耗自身至多四层流血,每消耗一层多{4 + level}伤害";
            mana = 2;
            Action use = () => {

            };
            this.use = use;  
        }
        if (card_name == "倾泻.光羽") {
            this.color = color;
            describe = $"{7 + 3 * level}伤害 消耗至多3层自愈,每1层加{6 + level * 2}伤害";
            mana = 1;
            Action use = () => {
                role["attack_count"] += 1;
                role["attack"] += 10;
                int healAmount = (int)((role["heal"] > 3) ? 3 : role["heal"]);
                role["attack"] += healAmount * (6 + 2 * level);
                role["heal"] -= healAmount;

            };
            this.use = use;  
        }
        if (card_name == "复苏.光羽") {
            this.color = color;
            describe = $"10伤害,每回复过一次生命值,加{1 + level}伤害";
            Action use = () => {
                role["mana"] += 1;
                role["heal"] += 1 + level;

            };
            this.use = use;  
        }
        if (card_name == "神助.光羽") {
            this.color = color;
            describe = $"10伤害,每回复过一次生命值,加{1 + level}伤害";
            Action use = () => {
                role["attack_count"] += 1;
                role["attack"] += 10;
                role["attack"] += role["recover_count"] * (1 + level);

            };
            this.use = use;  
        }   
        color = "红";
        if(card_name == "鏖战.蓄力") {
            this.color = color;

            if (level != 1)
            {
                describe = $"积蓄(20): {-50 + level * 50}伤害";
            }
            describe += $"魔阵:积蓄效果触发后将再次开始倒计时";
            this.broken = true;
            Action use = () => {
                role.again = true;
                if(level != 1) {
                    role["card_accumulate"] += 1;
                    role.accumulateList.Add($"20 > 20 > attack > const_num > {-50 + level * 50} > #FF0000 > 1");
                    role.accumulateList.Add($"20 > 20 > attack_count > const_num > 1 > none > 1");
                }
            };    
            this.use = use;      
        }
        if(card_name == "乐符狂热") {
            this.color = color;
            describe = $"乐符+{5 + 5 * level} 积蓄(5)获得等于乐符数量{0.5 + 0.5 * level}倍的力量";
            Action use = () => {
                role["card_accumulate"] += 1;
                role["note"] += 5 + 5 * level;
                role.accumulateList.Add($"5 > 5 > power > note > {0.5 + 0.5 * level} > #B2381E > 1");
            };    
            this.use = use;         
        }
        if(card_name == "音律强击") {
            this.color = color;
            describe = $"{5 + 5 * level} 受{5 + 5 * level}倍乐符与力量加成";
            Action use = () => {
                role["attack"] += 5 + 5 * level;
                role["attack"] += (role["note"] + role["power"]) * (4 + 5 * level);
                role["attack_count"] += 1;
                role["attack_note"] += 1;
            };
            this.use = use; 
        }
        color = "金";
        if(card_name == "争锋.蓄力") {
            this.color = color;
            describe = $"触发{1 + level}次场上的积蓄效果 迅捷";
            fast_card = true;
            Action use = () => {
                role.AccumulateAccelerate(accelerateNum:0, effectNum:1 + level, noLimit:true);
            };    
            this.use = use;         
        }
        if(card_name == "交续之时") {
            this.color = color;
            describe = $"生命+{(level == 1 ? 0 : 6)} 下一张牌连续生效{(level == 3 ? 3 : 2)}";
            Action use = () => {
                role["effect_count_next"] = (level == 3) ? 3 : 2;
            };       
            this.use = use;      
        }
        if(card_name == "奇攻.贮力") {
            this.color = color;
            if (level == 1) {
                role["mana"] = 2;
            }
            describe = $"复制{(level == 3 ? 2 : 1)}份场上的积蓄效果";
            Action use = () => {
                int num = (level == 3) ? 2 : 1;
                List<string> backup = new List<string>(role.accumulateList);
                for (int i = 0; i < num; i++)
                {
                    role.accumulateList.AddRange(backup);
                }                
            };  
            this.use = use;      
        }
        if(card_name == "闪攻.贮力") {
            this.color = color;
            describe = $"积蓄(10): {20 + 5 * level}伤害";
            Action use = () => {
                role["card_accumulate"] += 1;
                role.accumulateList.Add($"10 > 10 > attack > const_num > {20 + 5 * level} > #FFD700 > 1");
                role.accumulateList.Add($"10 > 10 > attack_count > const_num > 1 > none > 1");
            };      
            this.use = use;       
        }
        if(card_name == "速攻.贮力") {
            this.color = color;
            mana = 1;
            describe = $"加速{2 * level}次,由该卡触发的积蓄效果生效{1 + level}次";
            Action use = () => {
                role.AccumulateAccelerate(accelerateNum: 2 * level, effectNum: 1 + level);
            };       
            this.use = use;      
        }
        if(card_name == "乐符积蓄") {
            this.color = color;
            describe = $"积蓄(5):乐符+{15 + 5 * level}";
            Action use = () => {
                role["card_accumulate"] += 1;
                role.accumulateList.Add($"5 > 5 > note > const_num > {15 + 5 * level} > #A1BEDC > 1");
            };      
            this.use = use;       
        }
        if(card_name == "额外音符") {
            this.color = color;
            describe = $"魔阵:每回合乐符+{2 + level}";
            broken = true;
            Action use = () => {
                new Hook("turn_count", "note",  2 + level, "+", role);
            };     
            this.use = use;        
        }
        if(card_name == "回梦旋律") {
            this.color = color;
            describe = $"法力+{-1 + level * 2} 移除对方的护盾 迅捷";
            fast_card = true;
            Action use = () => {
                role["mana"] += -1 + level * 2;
                var enemy = role.process.role_list[(role.role_index + 1) % 2];
                enemy["shield"] = 0;
            };     
            this.use = use;       
        }
        if(card_name == "捷速谐乐") {
            this.color = color;
            mana = 3;
            describe = $"3伤害 * {4 + level}";
            Action use = () => {
                role["attack"] += 3 * (4 + level);
                role["attack_count"] += 4 + level;
                role["attack_note"] += 4 + level;
            };     
            this.use = use;        
        }
        if(card_name == "魔文乐谱") {
            this.color = color;
            mana = 1;
            describe = $"现有乐符数量增加{75 + 25 * level}%";
            Action use = () => {
                role["note"] += (int)(role["note"] * (75 + 25 * level) / 100);
            };       
            this.use = use;      
        }
        color = "紫";
        if(card_name == "激决.蓄力") {
            this.color = color;
            describe = $"场上每有一个积蓄效果, 获得{1 + level}层力量";
            Action use = () => {
                role["power"] += role.GetAccumulateNum() * (1 + level);
            };       
            this.use = use;      
        }
        if(card_name == "搏战.集力") {
            this.color = color;
            describe = $"法力+{-2 + level * 2}场上所有积蓄倒计时变为其中最小值 迅捷";
            fast_card = true;
            Action use = () => {
                if (role.GetAccumulateNum() > 0) {
                    int min = role.GetAccumulateMin();
                    role.AccumulateAccelerate(effectNum:0, min:min);                       
                }
            };     
            this.use = use;        
        }
        if(card_name == "破袭.集力") {
            this.color = color;
            describe = $"积蓄(8):{15 + 5 * level}";
            Action use = () => {
                role["card_accumulate"] += 1;
                role.accumulateList.Add($"8 > 8 > attack > const_num > {15 + 5 * level} > #AF6EF0 > 1");
            }; 
            this.use = use;            
        }
        if(card_name == "威势.集力") {
            this.color = color;
            describe = $"法力+{3 + level} 积蓄(7):生命+{2 + level * 3}";
            Action use = () => {
                role["mana"] += 3 + level;
                role["card_accumulate"] += 1;
                role.accumulateList.Add($"7 > 7 > life_recover > const_num > {2 + level * 3} > #008000 > 1");
            };     
            this.use = use;        
        }
        if(card_name == "瞬袭.集力") {
            this.color = color;
            mana = 2;
            describe = $"场上如果有2个及以上积蓄倒计时则造成{25 + level * 5}伤害";
            Action use = () => {
                if (role.GetAccumulateNum() >= 2) {
                    role["attack"] += 25 + level * 5;
                    role["attack_count"] += 1;
                    role["attack_note"] += 1;
                }
            };     
            this.use = use;        
        }
        if(card_name == "突袭.集力") {
            this.color = color;
            describe = $"回复等同于场上积蓄数量{4 + level}倍的生命";
            Action use = () => {
                role["life_recover"] += role.GetAccumulateNum() * (4 + level);
            };     
            this.use = use;        
        }
        if(card_name == "延时乐曲") {
            this.color = color;
            describe = $"乐符+{1 + 2 * level} 积蓄(5): 1伤害 * {4 + level}";
            Action use = () => {
                role["card_accumulate"] += 1;
                role["note"] += 1 + 2 * level;
                role.accumulateList.Add($"5 > 5 > attack > const_num > {4 + level} > #AF6EF0 > 1");
                role.accumulateList.Add($"5 > 5 > attack_count > const_num > {4 + level} > none > 1");
            };  
            this.use = use;           
        }
        if(card_name == "音符预演") {
            this.color = color;
            describe = $"乐符+{1 + 2 * level}下一次攻击后返还乐符";
            Action use = () => {
                role["note"] += 1 + 2 * level;
                role.return_note = true;
            };   
            this.use = use;          
        }
        if(card_name == "肆意.小调") {
            this.color = color;
            describe = $"法力+{(level == 3 ? 4 : 2)} 魔阵:每{(level == 1 ? 2 : 1)}回合加1法力";
            broken = true;
            Action use = () => {
                role["mana"] += level == 3 ? 4 : 2;
                new Hook("turn_count", "mana", (float)(level == 1 ? 0.5 : 1), "+", role);
            };    
            this.use = use;         
        }
        if(card_name == "安神.小调") {
            this.color = color;
            mana = 2;
            describe = $"{7 + 5 * level}伤害 对方法力-2";
            Action use = () => {
                var enemy = role.process.role_list[(role.role_index + 1) % 2];
                role["attack"] += 7 + 5 * level;
                enemy["mana"] -= 2;
                role["attack_count"] += 1;
                role["attack_note"] += 1;
            };       
            this.use = use;      
        }
        if(card_name == "甜美.小调") {
            this.color = color;
            broken = true;
            describe = $"魔阵:每获得一枚乐符加{level}护盾";
            Action use = () => {
                new Hook("note","shield", level * 30, "+", role);
            };      
            this.use = use;       
        }
        if(card_name == "四重.小调") {
            this.color = color;
            mana = 2;
            describe = $"2伤害*{3 + level}";
            Action use = () => {
                role["attack"] += 2 * (3 + level);
                role["attack_count"] += 3 + level;
                role["attack_note"] += 3 + level;
            };     
            this.use = use;
        }
        if(card_name == "激昂.小调") {
            this.color = color;
            describe = $"护盾+{4 + 4 * level} 获得等同于法力的乐符 至多{4 + 4 * level}";
            Action use = () => {
                role["shield"] += (4 + 4 * level) * 30;
                role["note"] += (4 + 4 * level) < role["mana"] ? (4 + 4 * level) : role["mana"];
            };    
            this.use = use;         
        }


























  
    }

}


public class GameProcess : MonoBehaviour
{
    
    public static List<string> starAndLuck = new()
    {
        "垒之护", "绚烂.星霞", "跃增.运时", "矛之突", "灼灼.星熠", "复加.得时", "幸运一掷", "时来运转", "好运加护",
        "魔阵.星数", "焰力.星数", "幻记.星数", "盾之守", "烁烁.星电", "倍加.巧时", "术士.魔阵", "骑士.魔运", "医者.魔运",
        "战车.魔运", "伤.星数", "灵.星数", "赐.星数", "势.星数"
    };

    public static List<string> lightAndNight = new()
    {
        "光羽夜蝶", "收割.夜蝶", "复原.光羽", "夜影光烁", "血疗.夜蝶", "神愈.光羽", "渴血.夜蝶",
        "血噬.夜蝶", "晨昏", "裂伤.夜蝶", "清心.光羽", "神罚.光羽", "神赐.光羽", "光夜交织",
        "血镰.夜蝶", "静心.光羽", "以血换血.夜蝶", "灵愈.光羽", "回愈.光羽", "狂宴.夜蝶",
        "拂晓", "刃舞.夜蝶", "血阵.夜蝶", "血清.夜蝶", "倾泻.光羽", "倾泻.光羽", "神助.光羽"
    };

    public static List<string> songAndLight = new()
    {
        "鏖战.蓄力", "乐符狂热", "音律强击", "争锋.蓄力", "交续之时", "奇攻.贮力", "闪攻.贮力",
        "速攻.贮力", "乐符积蓄", "额外音符", "回梦旋律", "捷速谐乐", "魔文乐谱", "激决.蓄力",
        "搏战.集力", "破袭.集力", "威势.集力", "瞬袭.集力", "突袭.集力", "延时乐曲", "音符预演",
        "肆意.小调", "安神.小调", "甜美.小调", "四重.小调", "激昂.小调"
    };
    public List<Roles> role_list = new();
    public System.Random random = new();
    public static List<List<string>> cardPacks = new();
    private static GameProcess instance;
    public static List<string> roleSelList = new();

    public GameProcess() {}
    public static GameProcess? Instance
    {
        get
        {
            if (instance == null)
            {
                UnityEngine.Debug.Log("Singleton instance has not been initialized.");
            }
            return instance;
        }
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); // 保持单例在场景切换时不被销毁
            InitializeSingleton();
        }
        else
        {
            Destroy(gameObject); // 如果已经存在其他实例，销毁这个
        }
    }

    private void InitializeSingleton()
    {
        // 单例初始化的逻辑，包括创建 card_pack1 和 card_pack2

        // 创建 Roles 实例
        if (BeginSel.modeSel == "offLine") {
            Roles role1 = new Roles(roleSelList[0], cardPacks[1], this);
            Roles role2 = new Roles(roleSelList[1], cardPacks[0], this);
            role1.RoleLoad();
            role2.RoleLoad();            
        }else {
            Roles role1 = new Roles(roleSelList[0], cardPacks[1], this);
            Roles role2 = new Roles(roleSelList[1], cardPacks[0], this);
            role1.RoleLoad();
            role2.RoleLoad();             
        }

    }

    public static List<T> ChooseRandomElements<T>(List<T> objects, int k)
    {
        List<T> result = new();
        System.Random random = new();

        if (k > objects.Count)
        {
            k = objects.Count;
        }
        for (int i = 0; i < k; i++)
        {
            int randomIndex = random.Next(0, objects.Count);
            T selectedObject = objects[randomIndex];
            result.Add(selectedObject);
        }

        return result;
    }

}


