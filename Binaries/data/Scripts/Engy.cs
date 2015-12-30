/* FactionCommand 1.0
[CONFIG]
title:Engy
time:10,
cost:10,
previewModel:none
wall:false
placeable:false
*/

MoveBehavior move; 

protected override void onInitialize() {
    Type = IntObjType.SUPPORT;

    IntObj2D.iconId = 0;
    IntObj2D.bracketSize=1;

    Health = 1+ BackgroundGame.random.Next(99);
    HealthMax = 100;

    move = new MoveBehavior(this);

    /*AddBehavior(new TargetBehavior(this));

    sensorBeh = AddBehavior(new SensorBehavior(this)) as SensorBehavior;

    moveBeh = (MoveBehavior)AddBehavior(new MoveBehavior(this));

    AddIntel(new IntelHandler(IntelType.Visible, 5));

    CalculateHeight();*/
        
}

protected override void onOrder() {
    switch(CurrentOrder.type){
        case OrderType.MOVE:
            move.Move(CurrentOrder as Order_Move);
            break;
    }
}

protected override void onUpdate() {
    if (CurrentOrder!=null) {
        switch (CurrentOrder.type) {
            case OrderType.MOVE:
                if (move.reached || move.unreachable) {
                    NextOrder();
                }
                break;
        }
    }
}