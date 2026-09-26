import bpy
import math
import os

def build_dragon_animations():
    print("=== Starting Dragon Animation Generation ===")
    
    # 1. Reset scene and import RedDragon_Optimized.fbx
    bpy.ops.wm.read_factory_settings(use_empty=True)
    fbx_path = os.path.abspath('Assets/Models/RedDragon/source/RedDragon_Optimized.fbx')
    bpy.ops.import_scene.fbx(filepath=fbx_path)
    
    arm = [obj for obj in bpy.data.objects if obj.type == 'ARMATURE'][0]
    bpy.context.view_layer.objects.active = arm
    
    # Clear any old animation data and remove legacy single-frame actions
    arm.animation_data_clear()
    for act in list(bpy.data.actions):
        bpy.data.actions.remove(act)
    
    # 2. Fix Armature bone hierarchy (reparent IK bones to FK skeleton)
    bpy.ops.object.mode_set(mode='EDIT')
    eb = arm.data.edit_bones
    
    if 'wingHand.L' in eb and 'lowerWing.L' in eb:
        eb['wingHand.L'].parent = eb['lowerWing.L']
    if 'wingHand.R' in eb and 'lowerWing.R' in eb:
        eb['wingHand.R'].parent = eb['lowerWing.R']
    if 'hand.L' in eb and 'forearm.L' in eb:
        eb['hand.L'].parent = eb['forearm.L']
    if 'hand.R' in eb and 'forearm.R' in eb:
        eb['hand.R'].parent = eb['forearm.R']
    if 'foot.L' in eb and 'ankle.L' in eb:
        eb['foot.L'].parent = eb['ankle.L']
    if 'foot.R' in eb and 'ankle.R' in eb:
        eb['foot.R'].parent = eb['ankle.R']
        
    bpy.ops.object.mode_set(mode='POSE')
    
    # Reset all pose bones to clean rest pose
    for pb in arm.pose.bones:
        pb.rotation_mode = 'XYZ'
        pb.location = (0, 0, 0)
        pb.rotation_euler = (0, 0, 0)
        pb.scale = (1, 1, 1)
    bpy.context.view_layer.update()
    
    print("Skeleton reparented and rest pose cleared.")
    
    # Helper to insert keyframe on bone rotation and location
    def kf_bone(pb, frame, rot=None, loc=None):
        if rot is not None:
            pb.rotation_euler = rot
            pb.keyframe_insert(data_path="rotation_euler", frame=frame)
        if loc is not None:
            pb.location = loc
            pb.keyframe_insert(data_path="location", frame=frame)
            
    # Base flight limb tuck angles
    # Front limbs tucked smoothly under ribs
    tuck_upperArm_L = (math.radians(-35), math.radians(15), math.radians(20))
    tuck_forearm_L = (math.radians(45), math.radians(-10), math.radians(0))
    tuck_hand_L = (math.radians(20), 0, math.radians(10))
    
    tuck_upperArm_R = (math.radians(-35), math.radians(-15), math.radians(-20))
    tuck_forearm_R = (math.radians(45), math.radians(10), math.radians(0))
    tuck_hand_R = (math.radians(20), 0, math.radians(-10))
    
    # Hind limbs trailing aerodynamically
    tuck_thigh_L = (math.radians(30), math.radians(-10), math.radians(15))
    tuck_lowerLeg_L = (math.radians(-40), 0, 0)
    tuck_ankle_L = (math.radians(20), 0, math.radians(10))
    tuck_foot_L = (math.radians(15), 0, 0)
    
    tuck_thigh_R = (math.radians(30), math.radians(10), math.radians(-15))
    tuck_lowerLeg_R = (math.radians(-40), 0, 0)
    tuck_ankle_R = (math.radians(20), 0, math.radians(-10))
    tuck_foot_R = (math.radians(15), 0, 0)
    
    def apply_tucked_limbs(frame):
        for name, rot in [
            ('upperArm.L', tuck_upperArm_L), ('forearm.L', tuck_forearm_L), ('hand.L', tuck_hand_L),
            ('upperArm.R', tuck_upperArm_R), ('forearm.R', tuck_forearm_R), ('hand.R', tuck_hand_R),
            ('thigh.L', tuck_thigh_L), ('lowerLeg.L', tuck_lowerLeg_L), ('ankle.L', tuck_ankle_L), ('foot.L', tuck_foot_L),
            ('thigh.R', tuck_thigh_R), ('lowerLeg.R', tuck_lowerLeg_R), ('ankle.R', tuck_ankle_R), ('foot.R', tuck_foot_R)
        ]:
            if name in arm.pose.bones:
                kf_bone(arm.pose.bones[name], frame, rot=rot)
                
    # =========================================================================
    # ACTION 1: Fly_Idle (40 frames, smooth hovering flight loop)
    # =========================================================================
    act_idle = bpy.data.actions.new(name="Fly_Idle")
    act_idle.use_frame_range = True
    act_idle.frame_start = 1
    act_idle.frame_end = 41
    arm.animation_data_create()
    arm.animation_data.action = act_idle
    
    total_frames = 40
    for f in range(1, total_frames + 2):
        t = (f - 1) / float(total_frames) # 0.0 to 1.0
        angle = t * 2.0 * math.pi
        
        # Wing flap phase:
        # Upstroke from t=0.0 to t=0.5, Downstroke from t=0.5 to t=1.0
        # Flap angle around X:
        flap_main = math.sin(angle) # -1 to +1
        flap_lag = math.sin(angle - 0.5)
        
        # Upper wing flap: +32 deg at top, -22 deg at bottom
        wing_elev = math.radians(5.0 + 27.0 * flap_main)
        wing_sweep_L = math.radians(8.0 * math.cos(angle))
        wing_sweep_R = math.radians(-8.0 * math.cos(angle))
        
        # Lower wing folds during upstroke (when flap_main > 0)
        elbow_fold = math.radians(12.0 + 16.0 * flap_lag)
        
        # Wing hand flex
        hand_flex = math.radians(8.0 + 12.0 * math.sin(angle - 1.0))
        
        # Apply to Left and Right wings
        kf_bone(arm.pose.bones['upperWing.L'], f, rot=(wing_elev, 0, wing_sweep_L))
        kf_bone(arm.pose.bones['upperWing.R'], f, rot=(wing_elev, 0, wing_sweep_R))
        
        kf_bone(arm.pose.bones['lowerWing.L'], f, rot=(elbow_fold, 0, 0))
        kf_bone(arm.pose.bones['lowerWing.R'], f, rot=(elbow_fold, 0, 0))
        
        kf_bone(arm.pose.bones['wingHand.L'], f, rot=(hand_flex, 0, 0))
        kf_bone(arm.pose.bones['wingHand.R'], f, rot=(hand_flex, 0, 0))
        
        # Wing membrane finger feathering
        finger_curl = math.radians(5.0 * math.sin(angle - 1.5))
        for finger in ['wingMiddle1.L', 'wingRing1.L', 'wingPinky1.L', 'wingMiddle1.R', 'wingRing1.R', 'wingPinky1.R']:
            if finger in arm.pose.bones:
                kf_bone(arm.pose.bones[finger], f, rot=(finger_curl, 0, 0))
                
        # Hover vertical bob on pelvis and chest
        hover_bob_z = 0.25 * math.cos(angle)
        kf_bone(arm.pose.bones['pelvis'], f, loc=(0, 0, hover_bob_z), rot=(math.radians(3.0 * flap_main), 0, 0))
        kf_bone(arm.pose.bones['chest'], f, rot=(math.radians(-2.0 * flap_main), 0, 0))
        
        # Counter-stabilizing head and neck (head stays rock-steady looking ahead)
        neck_pitch = math.radians(-1.5 * flap_main)
        for neck_bone in ['neck1', 'neck2', 'neck3', 'neck4', 'neck5']:
            if neck_bone in arm.pose.bones:
                kf_bone(arm.pose.bones[neck_bone], f, rot=(neck_pitch, 0, 0))
        if 'head' in arm.pose.bones:
            kf_bone(arm.pose.bones['head'], f, rot=(math.radians(2.0 * flap_main), 0, 0))
        if 'jaw' in arm.pose.bones:
            # Subtle breathing jaw parted ~5 deg
            kf_bone(arm.pose.bones['jaw'], f, rot=(math.radians(5.0 + 2.0 * math.sin(angle)), 0, 0))
            
        # Tail serpentine wave
        for i in range(1, 15):
            tbone = f'tail{i}'
            if tbone in arm.pose.bones:
                phase_lag = i * 0.42
                amp_mult = 1.0 + 0.12 * i
                # Horizontal sway (Z axis)
                sway_z = math.radians(amp_mult * 2.2 * math.sin(angle - phase_lag))
                # Vertical undulation (X axis)
                wave_x = math.radians(amp_mult * 1.1 * math.cos(angle - phase_lag))
                kf_bone(arm.pose.bones[tbone], f, rot=(wave_x, 0, sway_z))
                
        # Tucked limbs
        apply_tucked_limbs(f)
        
    print(f"Action 'Fly_Idle' generated ({act_idle.frame_end} frames).")

    # =========================================================================
    # ACTION 2: Fly_Forward (24 frames, faster high-speed forward flight)
    # =========================================================================
    act_fwd = bpy.data.actions.new(name="Fly_Forward")
    act_fwd.use_frame_range = True
    act_fwd.frame_start = 1
    act_fwd.frame_end = 25
    arm.animation_data.action = act_fwd
    
    total_frames = 24
    for f in range(1, total_frames + 2):
        t = (f - 1) / float(total_frames)
        angle = t * 2.0 * math.pi
        
        flap_main = math.sin(angle)
        flap_lag = math.sin(angle - 0.5)
        
        # Pitched body forward (-14 deg)
        kf_bone(arm.pose.bones['pelvis'], f, rot=(math.radians(-12.0 + 3.0 * flap_main), 0, 0), loc=(0, 0, 0.3 * math.cos(angle)))
        kf_bone(arm.pose.bones['chest'], f, rot=(math.radians(-5.0 - 2.0 * flap_main), 0, 0))
        
        # Powerful wing flaps with forward thrust
        wing_elev = math.radians(2.0 + 32.0 * flap_main)
        wing_thrust_L = math.radians(14.0 * math.cos(angle))
        wing_thrust_R = math.radians(-14.0 * math.cos(angle))
        
        kf_bone(arm.pose.bones['upperWing.L'], f, rot=(wing_elev, 0, wing_thrust_L))
        kf_bone(arm.pose.bones['upperWing.R'], f, rot=(wing_elev, 0, wing_thrust_R))
        
        kf_bone(arm.pose.bones['lowerWing.L'], f, rot=(math.radians(15.0 + 20.0 * flap_lag), 0, 0))
        kf_bone(arm.pose.bones['lowerWing.R'], f, rot=(math.radians(15.0 + 20.0 * flap_lag), 0, 0))
        
        kf_bone(arm.pose.bones['wingHand.L'], f, rot=(math.radians(10.0 + 16.0 * math.sin(angle - 0.8)), 0, 0))
        kf_bone(arm.pose.bones['wingHand.R'], f, rot=(math.radians(10.0 + 16.0 * math.sin(angle - 0.8)), 0, 0))
        
        # Head stretched forward aggressively
        for neck_bone in ['neck1', 'neck2', 'neck3', 'neck4', 'neck5']:
            if neck_bone in arm.pose.bones:
                kf_bone(arm.pose.bones[neck_bone], f, rot=(math.radians(3.0), 0, 0))
        if 'head' in arm.pose.bones:
            kf_bone(arm.pose.bones['head'], f, rot=(math.radians(6.0), 0, 0))
        if 'jaw' in arm.pose.bones:
            kf_bone(arm.pose.bones['jaw'], f, rot=(math.radians(14.0 + 3.0 * math.sin(angle)), 0, 0))
            
        # Trailing tail in high-energy slipstream
        for i in range(1, 15):
            tbone = f'tail{i}'
            if tbone in arm.pose.bones:
                phase_lag = i * 0.55
                amp_mult = 1.0 + 0.15 * i
                sway_z = math.radians(amp_mult * 3.2 * math.sin(angle - phase_lag))
                wave_x = math.radians(amp_mult * 1.5 * math.cos(angle - phase_lag))
                kf_bone(arm.pose.bones[tbone], f, rot=(wave_x, 0, sway_z))
                
        apply_tucked_limbs(f)
        
    print(f"Action 'Fly_Forward' generated ({act_fwd.frame_end} frames).")

    # =========================================================================
    # ACTION 3: Fire_Breath (60 frames, fierce breath attack)
    # =========================================================================
    act_fire = bpy.data.actions.new(name="Fire_Breath")
    act_fire.use_frame_range = True
    act_fire.frame_start = 1
    act_fire.frame_end = 60
    arm.animation_data.action = act_fire
    
    for f in range(1, 61):
        # 1-12: Windup (rear back, mouth opens wide)
        # 13-44: Fire blast (head thrust forward, jaw 52 deg open, sweeping fan)
        # 45-60: Recovery back to idle
        if f <= 12:
            prog = f / 12.0
            jaw_open = math.radians(52.0 * prog)
            neck_bend = math.radians(-22.0 * prog) # arch back
            chest_pitch = math.radians(10.0 * prog)
            head_yaw = 0
            wing_elev = math.radians(20.0 + 15.0 * math.sin(f * 0.4))
        elif f <= 44:
            prog = (f - 12) / 32.0
            jaw_open = math.radians(52.0 + 3.0 * math.sin(f * 1.2)) # fierce roar vibration
            neck_bend = math.radians(12.0) # lunged forward
            chest_pitch = math.radians(-8.0)
            # Sweeping fire stream left to right
            head_yaw = math.radians(14.0 * math.sin((f - 12) * 0.22))
            wing_elev = math.radians(10.0 + 25.0 * math.sin(f * 0.8)) # rapid flutter hover
        else:
            prog = (f - 44) / 16.0
            jaw_open = math.radians(52.0 * (1.0 - prog))
            neck_bend = math.radians(12.0 * (1.0 - prog))
            chest_pitch = math.radians(-8.0 * (1.0 - prog))
            head_yaw = math.radians(14.0 * math.sin((44 - 12) * 0.22) * (1.0 - prog))
            wing_elev = math.radians(5.0 + 27.0 * math.sin(f * 0.3))
            
        kf_bone(arm.pose.bones['jaw'], f, rot=(jaw_open, 0, 0))
        kf_bone(arm.pose.bones['head'], f, rot=(neck_bend * 0.5, 0, head_yaw))
        for neck_bone in ['neck1', 'neck2', 'neck3', 'neck4', 'neck5']:
            if neck_bone in arm.pose.bones:
                kf_bone(arm.pose.bones[neck_bone], f, rot=(neck_bend * 0.3, 0, head_yaw * 0.2))
        kf_bone(arm.pose.bones['chest'], f, rot=(chest_pitch, 0, 0))
        kf_bone(arm.pose.bones['pelvis'], f, rot=(chest_pitch * 0.5, 0, 0))
        
        # Wings fluttering to brace recoil
        kf_bone(arm.pose.bones['upperWing.L'], f, rot=(wing_elev, 0, math.radians(10.0)))
        kf_bone(arm.pose.bones['upperWing.R'], f, rot=(wing_elev, 0, math.radians(-10.0)))
        kf_bone(arm.pose.bones['lowerWing.L'], f, rot=(math.radians(18.0), 0, 0))
        kf_bone(arm.pose.bones['lowerWing.R'], f, rot=(math.radians(18.0), 0, 0))
        kf_bone(arm.pose.bones['wingHand.L'], f, rot=(math.radians(12.0), 0, 0))
        kf_bone(arm.pose.bones['wingHand.R'], f, rot=(math.radians(12.0), 0, 0))
        
        # Tail bracing
        for i in range(1, 15):
            tbone = f'tail{i}'
            if tbone in arm.pose.bones:
                sway_z = math.radians(1.5 * i * math.sin(f * 0.15))
                kf_bone(arm.pose.bones[tbone], f, rot=(math.radians(-1.0 * i), 0, sway_z))
                
        apply_tucked_limbs(f)
        
    print(f"Action 'Fire_Breath' generated ({act_fire.frame_end} frames).")

    # =========================================================================
    # ACTION 4: Tail_Whip (45 frames, 180-degree sweep strike)
    # =========================================================================
    act_tail = bpy.data.actions.new(name="Tail_Whip")
    act_tail.use_frame_range = True
    act_tail.frame_start = 1
    act_tail.frame_end = 45
    arm.animation_data.action = act_tail
    
    for f in range(1, 46):
        # 1-14: Coil left like a spring
        # 15-24: Explosive whip strike sweeping right
        # 25-45: Decelerate and recover
        if f <= 14:
            prog = f / 14.0
            # Body twists left
            pelvis_yaw = math.radians(-35.0 * prog)
            # Tail coils tightly left
            tail_curve = math.radians(-8.0 * prog)
            wing_tilt = math.radians(-20.0 * prog)
        elif f <= 24:
            prog = (f - 14) / 10.0
            # Explosive snap from -35 deg to +55 deg
            pelvis_yaw = math.radians(-35.0 + 90.0 * prog)
            # Tail snaps violently from -8 deg per bone to +12 deg per bone
            tail_curve = math.radians(-8.0 + 20.0 * prog)
            wing_tilt = math.radians(-20.0 + 45.0 * prog)
        else:
            prog = (f - 24) / 21.0
            pelvis_yaw = math.radians(55.0 * (1.0 - prog))
            tail_curve = math.radians(12.0 * (1.0 - prog))
            wing_tilt = math.radians(25.0 * (1.0 - prog))
            
        kf_bone(arm.pose.bones['pelvis'], f, rot=(0, 0, pelvis_yaw))
        kf_bone(arm.pose.bones['chest'], f, rot=(0, 0, pelvis_yaw * 0.6))
        
        # Tail segments cascade the whip curve with progressive dynamic whip lag
        for i in range(1, 15):
            tbone = f'tail{i}'
            if tbone in arm.pose.bones:
                # Progressive multiplier: tail tip whips hardest!
                tip_boost = 1.0 + (i / 14.0) * 1.6
                kf_bone(arm.pose.bones[tbone], f, rot=(0, 0, tail_curve * tip_boost))
                
        # Wings bank dynamically with body twist
        kf_bone(arm.pose.bones['upperWing.L'], f, rot=(math.radians(15.0) + wing_tilt, 0, 0))
        kf_bone(arm.pose.bones['upperWing.R'], f, rot=(math.radians(15.0) - wing_tilt, 0, 0))
        kf_bone(arm.pose.bones['lowerWing.L'], f, rot=(math.radians(20.0), 0, 0))
        kf_bone(arm.pose.bones['lowerWing.R'], f, rot=(math.radians(20.0), 0, 0))
        kf_bone(arm.pose.bones['wingHand.L'], f, rot=(math.radians(15.0), 0, 0))
        kf_bone(arm.pose.bones['wingHand.R'], f, rot=(math.radians(15.0), 0, 0))
        
        apply_tucked_limbs(f)
        
    print(f"Action 'Tail_Whip' generated ({act_tail.frame_end} frames).")

    # =========================================================================
    # ACTION 5: Dive_Bomb (45 frames, high-speed dive and swoop)
    # =========================================================================
    act_dive = bpy.data.actions.new(name="Dive_Bomb")
    act_dive.use_frame_range = True
    act_dive.frame_start = 1
    act_dive.frame_end = 45
    arm.animation_data.action = act_dive
    
    for f in range(1, 46):
        # 1-10: Pitch down into steep dive
        # 11-25: Wings tucked sleek, high velocity plunge
        # 26-45: Pull up nose, wings spread wide to brake
        if f <= 10:
            prog = f / 10.0
            pitch = math.radians(-55.0 * prog)
            wing_tuck = prog # 0 to 1
        elif f <= 25:
            pitch = math.radians(-55.0)
            wing_tuck = 1.0
        else:
            prog = (f - 25) / 20.0
            # Pull up nose from -55 deg past horizontal (+15 deg) and settle to 0
            if prog < 0.5:
                sub_p = prog / 0.5
                pitch = math.radians(-55.0 + 70.0 * sub_p)
            else:
                sub_p = (prog - 0.5) / 0.5
                pitch = math.radians(15.0 * (1.0 - sub_p))
            wing_tuck = 1.0 - prog
            
        kf_bone(arm.pose.bones['pelvis'], f, rot=(pitch, 0, 0))
        kf_bone(arm.pose.bones['chest'], f, rot=(pitch * 0.3, 0, 0))
        
        # Wings tucking tightly against body during plunge
        wing_elev = math.radians(15.0 * (1.0 - wing_tuck) - 30.0 * wing_tuck)
        lower_wing_fold = math.radians(15.0 * (1.0 - wing_tuck) + 55.0 * wing_tuck)
        hand_tuck = math.radians(10.0 * (1.0 - wing_tuck) + 40.0 * wing_tuck)
        
        kf_bone(arm.pose.bones['upperWing.L'], f, rot=(wing_elev, 0, math.radians(25.0 * wing_tuck)))
        kf_bone(arm.pose.bones['upperWing.R'], f, rot=(wing_elev, 0, math.radians(-25.0 * wing_tuck)))
        kf_bone(arm.pose.bones['lowerWing.L'], f, rot=(lower_wing_fold, 0, 0))
        kf_bone(arm.pose.bones['lowerWing.R'], f, rot=(lower_wing_fold, 0, 0))
        kf_bone(arm.pose.bones['wingHand.L'], f, rot=(hand_tuck, 0, 0))
        kf_bone(arm.pose.bones['wingHand.R'], f, rot=(hand_tuck, 0, 0))
        
        # Tail streamlined straight back
        for i in range(1, 15):
            tbone = f'tail{i}'
            if tbone in arm.pose.bones:
                kf_bone(arm.pose.bones[tbone], f, rot=(0, 0, 0))
                
        apply_tucked_limbs(f)
        
    print(f"Action 'Dive_Bomb' generated ({act_dive.frame_end} frames).")

    # =========================================================================
    # ACTION 6: Take_Hit (18 frames, flinch reaction)
    # =========================================================================
    act_hit = bpy.data.actions.new(name="Take_Hit")
    act_hit.use_frame_range = True
    act_hit.frame_start = 1
    act_hit.frame_end = 18
    arm.animation_data.action = act_hit
    
    for f in range(1, 19):
        if f <= 5:
            prog = f / 5.0
            recoil = prog
        else:
            prog = (f - 5) / 13.0
            recoil = 1.0 - prog
            
        kf_bone(arm.pose.bones['chest'], f, rot=(math.radians(22.0 * recoil), 0, math.radians(-10.0 * recoil)))
        kf_bone(arm.pose.bones['head'], f, rot=(math.radians(18.0 * recoil), 0, math.radians(12.0 * recoil)))
        if 'jaw' in arm.pose.bones:
            kf_bone(arm.pose.bones['jaw'], f, rot=(math.radians(28.0 * recoil), 0, 0))
        kf_bone(arm.pose.bones['upperWing.L'], f, rot=(math.radians(35.0 * recoil), 0, math.radians(20.0 * recoil)))
        kf_bone(arm.pose.bones['upperWing.R'], f, rot=(math.radians(35.0 * recoil), 0, math.radians(-20.0 * recoil)))
        apply_tucked_limbs(f)
        
    print(f"Action 'Take_Hit' generated ({act_hit.frame_end} frames).")

    # =========================================================================
    # ACTION 7: Die (50 frames, death collapse)
    # =========================================================================
    act_die = bpy.data.actions.new(name="Die")
    act_die.use_frame_range = True
    act_die.frame_start = 1
    act_die.frame_end = 50
    arm.animation_data.action = act_die
    
    for f in range(1, 51):
        if f <= 15:
            prog = f / 15.0
            # Initial convulsion / head slumps
            head_drop = math.radians(-35.0 * prog)
            body_tilt = math.radians(15.0 * prog)
            wing_limp = prog * 0.4
        elif f <= 38:
            prog = (f - 15) / 23.0
            head_drop = math.radians(-35.0 - 25.0 * prog)
            body_tilt = math.radians(15.0 + 35.0 * prog)
            wing_limp = 0.4 + prog * 0.6
        else:
            head_drop = math.radians(-60.0)
            body_tilt = math.radians(50.0)
            wing_limp = 1.0
            
        kf_bone(arm.pose.bones['head'], f, rot=(head_drop, 0, math.radians(-18.0 * wing_limp)))
        for neck_bone in ['neck1', 'neck2', 'neck3', 'neck4', 'neck5']:
            if neck_bone in arm.pose.bones:
                kf_bone(arm.pose.bones[neck_bone], f, rot=(head_drop * 0.2, 0, 0))
        if 'jaw' in arm.pose.bones:
            kf_bone(arm.pose.bones['jaw'], f, rot=(math.radians(38.0 * wing_limp), 0, 0))
            
        kf_bone(arm.pose.bones['pelvis'], f, rot=(0, body_tilt, 0), loc=(0, 0, -1.2 * wing_limp))
        
        # Wings fold limp and flop
        kf_bone(arm.pose.bones['upperWing.L'], f, rot=(math.radians(45.0 * wing_limp), 0, math.radians(35.0 * wing_limp)))
        kf_bone(arm.pose.bones['upperWing.R'], f, rot=(math.radians(45.0 * wing_limp), 0, math.radians(-35.0 * wing_limp)))
        kf_bone(arm.pose.bones['lowerWing.L'], f, rot=(math.radians(55.0 * wing_limp), 0, 0))
        kf_bone(arm.pose.bones['lowerWing.R'], f, rot=(math.radians(55.0 * wing_limp), 0, 0))
        kf_bone(arm.pose.bones['wingHand.L'], f, rot=(math.radians(30.0 * wing_limp), 0, 0))
        kf_bone(arm.pose.bones['wingHand.R'], f, rot=(math.radians(30.0 * wing_limp), 0, 0))
        
        # Tail goes completely limp hanging down
        for i in range(1, 15):
            tbone = f'tail{i}'
            if tbone in arm.pose.bones:
                kf_bone(arm.pose.bones[tbone], f, rot=(math.radians(-2.5 * i * wing_limp), 0, math.radians(1.2 * i * wing_limp)))
                
        apply_tucked_limbs(f)
        
    print(f"Action 'Die' generated ({act_die.frame_end} frames).")

    # Set default action to Fly_Idle
    arm.animation_data.action = act_idle
    
    # 3. Export FBX with all actions baked and embedded
    export_path = os.path.abspath('Assets/Models/RedDragon/source/RedDragon_Optimized.fbx')
    print(f"Exporting FBX with all 7 animation actions to: {export_path}")
    
    bpy.ops.export_scene.fbx(
        filepath=export_path,
        use_selection=False,
        bake_anim=True,
        bake_anim_use_all_bones=True,
        bake_anim_use_nla_strips=False,
        bake_anim_use_all_actions=True,
        bake_anim_force_startend_keying=True,
        bake_anim_step=1.0,
        bake_anim_simplify_factor=0.0,
        add_leaf_bones=False,
        primary_bone_axis='Y',
        secondary_bone_axis='X',
        axis_forward='-Z',
        axis_up='Y'
    )
    print("Export complete!")

if __name__ == '__main__':
    build_dragon_animations()
