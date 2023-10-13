public class AmmoItem : Item
{
    int AmmoGiven = 50;
    public override void Pick(FPSController Player)
    {
        Player.AddAmmo(AmmoGiven);
        Destroy(gameObject);
    }
}
