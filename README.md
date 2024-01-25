# __DESCRIPTION__ :

This is our own Unity Stats Handler package. Please take note that even through we made it public, it is intended to our own projects, therefore might not fit yours.

It includes: 

- A Stats Container
- A Stats Handler to retrieve and modify stats
- Stats Modifiers
- Affiliation (team) modifiers
- Attributes modifiers

Here's an example of a base stats SO containing a player's stats.
![image](https://github.com/BalD1/com.stdnounou.stats-system/assets/24933826/e15d8e1a-c11c-461d-8e42-5fc8a75f49a3)    
Let's look a this step by step:    
- Affiliations:    
![image](https://github.com/BalD1/com.stdnounou.stats-system/assets/24933826/7ff45bf7-43eb-4fd5-a81b-42cd4e8ebcfe)    
Here, you can see the player's affiliation. In here, you can define if you want to prevent your player from interacting with other affiliations (here the players can't interact with each other), or define stats modifiers. In this example, the players will inflict 2x damages to props. A stats SO can only contain one affiliation at a time.    
- Attributes:    
![image](https://github.com/BalD1/com.stdnounou.stats-system/assets/24933826/1e8ddba4-ea48-4db9-9bd4-ca1054518441)    
A stats SO can however contain as much attributes as you want. You can have duplicates attributes. Let's say we have 3 attributes : Fire, Water & Plant.    
Here, we can define stats modificators from other attributes in the attribute SO. Modificators defined here are from RECEIVED stats only.    
So here, our Plant attribute will:    
- Receive /2 damages from Water attributes
- Receive /1.5 damages from Plant attributes
- Receive x2 damages from Fire attributes
- Stats:    
![image](https://github.com/BalD1/com.stdnounou.stats-system/assets/24933826/b905bb27-6bfc-41e5-a723-eecaf4c5e851)    
These are the base stats of your object. Next to each stat key, we can see 3 values.
- Lowest Allowed Value: the absolute lowest value you want your stat to reach through modificators
- Value: the actual base value
- Highest Allowed Value: the absolute highest value you want your stat to reach through modificators


# __REQUIREMENTS__ :
This package contains a StatsHandler, which uses a dictionary to rapidly and easily retrieve a stat from a given object, with errors handling.    
Obviously, this requires keys to retrieve the stat. I chose an enum for this. However, since assets in the packages folder can (should) not be modified, I figured the better way would be to let the enum exist in the assets folder.    
So you'll need to manually add the enum into your assets folder :
- Either use [this package](https://github.com/BalD1/com.stdnounou.assets-creator.git)
  => Then click on this button, and let unity recompile ![image](https://github.com/BalD1/com.stdnounou.stats-system/assets/24933826/45adca3f-c826-4087-a181-d9a5e5f56666)    

   
- Or manually add these files in the same folder, separated from your other scripts :
## StatsKeys.cs
```csharp
namespace StdNounou.Stats 
{
    public enum E_StatsKeys
    {
        Health,
        Damages,
        DamageReduction,
        CritChances,
        CritMultiplier,
        AttackCooldown,
        Speed,
        Weight,
        KnockbackForce,
    }
}
```
## StdNounou.stats.runtime_ref
```
{
    "reference": "GUID:6ddfdff7ac37b354883336b81ebb5114"
}
```

# __DEPENDECIES__ :

-StdNounou custom core : https://github.com/BalD1/com.stdnounou.unity-custom-core    
-StdNounou tick manager : https://github.com/BalD1/com.stdnounou.unity-tick-manager
-Ayellowpaper's Serialized Dictionary: https://github.com/ayellowpaper/SerializedDictionary.git
