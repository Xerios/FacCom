ParticleFX trailz;
Vector3 lastPos,newPos;

protected override void onInit() {
    trailz = ForegroundGame.effectsMgr.Add("Exp2");
    trailz.Init(position);
    newPos = position;
}

protected override void onDispose() {
    trailz.Die();
    ParticleFX ffx = ForegroundGame.effectsMgr.Add("Flash");
    ffx.DeleteOnDeath=true;
    ffx.Init(target.currentPos);
	target.Damage(this, 20);
}

float timer=0;
protected override bool onUpdate() {
    timer+=0.1f;
    lastPos = newPos;
    newPos = Vector3.Lerp(position,target.pos,timer);
	return (timer>1.1f);
}

protected override void onRender() {
    trailz.pos = Vector3.Lerp(lastPos,newPos,ForegroundGame.timeInterpolateDelta);
}