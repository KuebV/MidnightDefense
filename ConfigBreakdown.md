# Configuration Breakdown
Indended to breakdown each one of the config variables into more understandable bits of information

## Friendly-Fire
```yml
f_f_detection: true
```
> Friendly-Fire Detection includes SCP-Teamkilling & Teamkilling when Server Friendly-Fire is disabled. There is currently no option to have either one disabled or enabled, this may change in the future
```yml
negate_f_f_damage: true
```
> As of v1.2.5, this does not negate all damage due to how SCP handles Friendly-Fire when a player is shot. However, if a player is about to die to cheaters, it will not allow them to die

```yml
f_f_detection_points: 1
```
> If Friendly-Fire is detected, this is how many points is added to the players current detection points

## SCP-049 Movement Cheats
```yml
s_c_p049_detection: true
```
> SCP-049 Movement Cheats only checks if the start zombie recall is the same position as when the player is eventually revived. Midnight allows cheaters to actively move around while reviving players. 

```yml
s_c_p049_detected_movement_points: 1
```
> If SCP-049 Movement is detected, this is how many points will be added to the players current detection points

## Infinite Range
```yml
infinite_range_detection: true
```
> If the cheater is playing as an SCP, Midnight allows them to hit players from a seemingly infinite distance away. This includes SCP's such as, SCP-049, 939, and 096

```yml
range_distance: 5
```
> The maximum allowed range for SCP's to be allowed to attack from

```yml
negate_infinite_range_damage: true
```
> If a player is hit from outside of this range, the damage will be negated

```yml
infinite_range_detection_points: 1
```
> If Infinite Range is detected, this is how many points will be added to the players current detection points

## Silent Aimbot
```yml
silent_aimbot_detection: true
```
> Currently of Release v1.2.5, Silent Aimbot Detection can only be enabled manually by a command. But in simple terms, it will spawn an NPC near the detected player and will constantly move around their crosshair randomly


```yml
silent_aimbot_detection_points: 1
```
> If Silent-Aimbot is detected, this is how many points will be added to the players current detection points

```yml
silent_aimbot_player_size:
- 1
- 1
- 1
```
> The scale of the NPC teleporting randomly around the players crosshair

```yml
disconnect_on_anti_aimbot_hit: false
```
> This was a bug turned feature, if anyone (including people who aren't even being detected) shoots the NPC, it will kick them saying they lost connection. It's advised to keep this disabled unless you are absolutely sure no one will shoot it

```yml
silent_aimbot_hit_threshold: 8
```
> How many times does the detected player allowed to shoot the NPC before it adds a detection point

```yml
silent_aimbot_teleport_time: 0.15
```
> How often does the NPC teleport around the detected player's crosshair? Lower values may consume more bandwidth if used for a long duration

```yml
silent_aimbot_trigger_report_amount: 3
```
> **This is currently not implemented, however. If a player receives the specified amount of reports, it will trigger the Anti-Aimbot NPC**


## Speedhack
```yml
speedhack_detection: true
```
> Speedhack includes guns firing at a faster rate than normally should be possible

```yml
speedhack_detection_threshold: 75
```
> The fastest time inbetween shots that is allowed, anything above 80 is risky

```yml
speedhack_detection_cancel_event: false
```
> If the detected player triggers speedhack, then should their shots just be cancelled

## NoClip
```yml
noclip_detection: true
```
> Cheaters are able to toggle noclip on and off without permission from the server.

```yml
noclip_detection_points: 1
```
> If NoClip is detected, this is how many points will be added to the players current detection points

```yml
noclip_rubberband: true
```
> If you attempt going any faster than should be physically possible, should you be teleported back to a point where you are safe?

```yml
noclip_maximum_velocity: 7.57189512
```
> Do not, and I repeat, do not set this value any lower than what it already is. Midnight Defense uses a custom velocity calculation to determine velocity. You can set it higher, but be aware cheaters may be able to move faster

```yml
no_clip_detection_speed: 0.5
```
> How often the plugin checks if a player may be noclipping

```yml
no_clip_detection_staff: false
```
> Should it detect staff members? I personally wouldn't enable this unless you never want to use noclip again. This will be eventually fixed with a new system

```yml
no_clip_allowed_roles:
- Tutorial
```
> If you want a player who is non-staff but you want them to be able to NoClip around, you would set them to a tutorial

## Miscellaneous / Staff
```yml
point_threshold: 10
```
> The amount of current detection points that is required in order to alert staff and discord

```yml
alert_online_staff: true
```
> If any staff are online, it will send the cheater alert to them

```yml
alert_max_times: 3
```
> Maximum times staff and discord can be alerted about the cheater

```yml
alert_timeframe: 10
```
> How often does the plugin check for detection points on players

```yml
discord_webhook_enabled: false
```
> If the discord webhook is enabled
















