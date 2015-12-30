
protected override void onInitialize() {
    Type = IntObjType.SUPPORT;

    IntObj2D.iconId = 1;
    IntObj2D.bracketSize=2;

    Health = 100;
    HealthMax = 100;

}

protected override void onOrder() {
    switch(CurrentOrder.type){
        case OrderType.BUILD:
            break;
    }
}

protected override void onUpdate() {
    if (CurrentOrder!=null) {
        switch (CurrentOrder.type) {
            case OrderType.BUILD:
                NextOrder();
                break;
        }
    }
}