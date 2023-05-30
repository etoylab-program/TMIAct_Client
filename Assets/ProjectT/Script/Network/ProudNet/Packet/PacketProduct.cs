using System.Collections.Generic;
using Nettention.Proud;


public class PacketProductBase : PacketBase
{
    // 클라이언트서 서버로 요청 보내는 오브젝트
    protected ProductC2S.Proxy m_C2SProxy = new ProductC2S.Proxy();
    // 서버에서 응답을 받는 오브젝트
    protected ProductS2C.Stub m_S2CStub = new ProductS2C.Stub();

    public ProductS2C.Stub Recv() { return m_S2CStub; }

    public override void DoAttach(NetClient _netClient) { return; }

    // 슬롯 관련
    public virtual void ReqAddItemSlot(System.UInt16 _addCnt) { Debug.LogError("Not Make embody - ReqAddItemSlot") ; }
    // 카드(서포터) 관련
    public virtual void ReqApplyPosCard(PktInfoCardApplyPos _pkt) { Debug.LogError("Not Make embody - ReqApplyPosCard") ; }
    public virtual void ReqApplyOutPosCard(System.UInt64 _uid) { Debug.LogError("Not Make embody - ReqApplyOutPosCard"); }
    public virtual void ReqSellCard(PktInfoCardSell _pkt) { Debug.LogError("Not Make embody - ReqSellCard"); }
    public virtual void ReqSetLockCard(PktInfoCardLock _pkt) { Debug.LogError("Not Make embody - ReqSetLockCard"); }
    public virtual void ReqChangeTypeCard(PktInfoCardTypeChangeReq _pkt) { Debug.LogError("Not Make embody - ReqChangeTypeCard"); }
    public virtual void ReqLvUpCard(PktInfoProductComGrowReq _pkt) { Debug.LogError("Not Make embody - ReqLvUpCard"); }
    public virtual void ReqSkillLvUpCard(PktInfoProductComGrowReq _pkt) { Debug.LogError("Not Make embody - ReqSkillLvUpCard"); }
    public virtual void ReqWakeCard(PktInfoProductComGrowReq _pkt) { Debug.LogError("Not Make embody - ReqWakeCard"); }
    public virtual void ReqEnchantCard(PktInfoProductComGrowReq _pkt) { Debug.LogError("Not Make embody - ReqEnchantCard"); }
    public virtual void ReqFavorLvRewardCard(PktInfoBookOnStateReq _pkt) { Debug.LogError("Not Make embody - ReqFavorLvRewardCard"); }

    // 아이템 관련
    public virtual void ReqSellItem(PktInfoItemSell _pkt) { Debug.LogError("Not Make embody - ReqSellItem"); }
    public virtual void ReqItemExchangeCash(PktInfoItemTIDCnt _pkt) { Debug.LogError("Not Make embody - ReqItemExchangeCash"); }
    public virtual void ReqUseItem(PktInfoUseItemReq _pkt) { Debug.LogError("Not Make embody - ReqUseItem"); }
    public virtual void ReqUseItemGoods(PktInfoItemCnt _pkt) { Debug.LogError("Not Make embody - ReqUseItem"); }

    // 곡옥 관련
    public virtual void ReqSellGem(PktInfoGemSell _pkt) { Debug.LogError("Not Make embody - ReqSellGem"); }
    public virtual void ReqSetLockGem(PktInfoGemLock _pkt) { Debug.LogError("Not Make embody - ReqSetLockGem"); }
    public virtual void ReqResetOptGem(PktInfoGemResetOptReq _pkt) { Debug.LogError("Not Make embody - ReqResetOptGem"); }
    public virtual void ReqResetOptSelectGem(PktInfoGemResetOptSelect _pkt) { Debug.LogError("Not Make embody - ReqResetOptSelectGem"); }
    public virtual void ReqLvUpGem(PktInfoProductComGrowReq _pkt) { Debug.LogError("Not Make embody - ReqLvUpGem"); }
    public virtual void ReqWakeGem(PktInfoProductComGrowReq _pkt) { Debug.LogError("Not Make embody - ReqWakeGem"); }    
	public virtual void ReqEvolutionGem(PktInfoProductComGrowReq _pkt) { Debug.LogError("Not Make embody - ReqEvolutionGem"); }
	public virtual void ReqAnalyzeGem(PktInfoProductComGrowReq _pkt) { Debug.LogError("Not Make embody - ReqAnalyzeGem"); }

	// 무기 관련
	public virtual void ReqSellWeapon(PktInfoWeaponSell _pkt) { Debug.LogError("Not Make embody - ReqSellWeapon"); }
    public virtual void ReqSetLockWeapon(PktInfoWeaponLock _pkt) { Debug.LogError("Not Make embody - ReqSetLockWeapon"); }
    public virtual void ReqLvUpWeapon(PktInfoProductComGrowReq _pkt) { Debug.LogError("Not Make embody - ReqLvUpWeapon"); }
    public virtual void ReqSkillLvUpWeapon(PktInfoProductComGrowReq _pkt) { Debug.LogError("Not Make embody - ReqSkillLvUpWeapon"); }
    public virtual void ReqWakeWeapon(PktInfoProductComGrowReq _pkt) { Debug.LogError("Not Make embody - ReqWakeWeapon"); }
    public virtual void ReqEnchantWeapon(PktInfoProductComGrowReq _pkt) { Debug.LogError("Not Make embody - ReqEnchantWeapon"); }
    public virtual void ReqApplyGemInWeapon(PktInfoWeaponSlotGem _pkt) { Debug.LogError("Not Make embody - ReqApplyGemInWeapon"); }
    public virtual void ReqAddSlotInWpnDepot(System.Byte _addCnt) { Debug.LogError("Not Make embody - ReqAddSlotInWpnDepot"); }
    public virtual void ReqApplySlotInWpnDepot(PktInfoWpnDepotApply _pkt) { Debug.LogError("Not Make embody - ReqApplySlotInWpnDepot"); }

    // 문양 관련
    public virtual void ReqApplyPosBadge(PktInfoBadgeApplyPos _pkt) { Debug.LogError("Not Make embody - ReqApplyPosBadge"); }
    public virtual void ReqApplyOutPosBadge(PktInfoBadgeComReq _pkt) { Debug.LogError("Not Make embody - ReqApplyOutPosBadge"); }
    public virtual void ReqSellBadge(PktInfoBadgeSell _pkt) { Debug.LogError("Not Make embody - ReqSellBadge"); }
    public virtual void ReqSetLockBadge(PktInfoBadgeLock _pkt) { Debug.LogError("Not Make embody - ReqSetLockBadge"); }
    public virtual void ReqUpgradeBadge(PktInfoBadgeComReq _pkt) { Debug.LogError("Not Make embody - ReqUpgradeBadge"); }
    public virtual void ReqResetUpgradeBadge(PktInfoBadgeComReq _pkt) { Debug.LogError("Not Make embody - ReqResetUpgradeBadge"); }

    // 분해 관련 
    public virtual void ReqDecomposition(PktInfoDecompositionReq _pkt) { Debug.LogError("Not Make embody - ReqDecomposition"); }
}
public class PacketProduct : PacketProductBase
{
    public override void DoAttach(NetClient _netClient)
    {
        _netClient.AttachProxy(m_C2SProxy);
        _netClient.AttachStub(m_S2CStub);

        return;
    }

    // 슬롯 관련
    public override void ReqAddItemSlot(System.UInt16 _addCnt) {
        m_C2SProxy.ReqAddItemSlot(HostID.HostID_Server, RmiContext.ReliableSend, _addCnt);
    }
    // 카드(서포터) 관련
    public override void ReqApplyPosCard(PktInfoCardApplyPos _pkt) {
        m_C2SProxy.ReqApplyPosCard(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqApplyOutPosCard(System.UInt64 _uid) {
        m_C2SProxy.ReqApplyOutPosCard(HostID.HostID_Server, RmiContext.ReliableSend, _uid);
    }
    public override void ReqSellCard(PktInfoCardSell _pkt) {
        m_C2SProxy.ReqSellCard(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqSetLockCard(PktInfoCardLock _pkt) {
        m_C2SProxy.ReqSetLockCard(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqChangeTypeCard(PktInfoCardTypeChangeReq _pkt) {
        m_C2SProxy.ReqChangeTypeCard(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqLvUpCard(PktInfoProductComGrowReq _pkt) {
        m_C2SProxy.ReqLvUpCard(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqSkillLvUpCard(PktInfoProductComGrowReq _pkt) {
        m_C2SProxy.ReqSkillLvUpCard(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqWakeCard(PktInfoProductComGrowReq _pkt) {
        m_C2SProxy.ReqWakeCard(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqEnchantCard(PktInfoProductComGrowReq _pkt) {
        m_C2SProxy.ReqEnchantCard(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqFavorLvRewardCard(PktInfoBookOnStateReq _pkt) {
        m_C2SProxy.ReqFavorLvRewardCard(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    // 아이템 관련
    public override void ReqSellItem(PktInfoItemSell _pkt) {
        m_C2SProxy.ReqSellItem(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqItemExchangeCash(PktInfoItemTIDCnt _pkt) {
        m_C2SProxy.ReqItemExchangeCash(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqUseItem(PktInfoUseItemReq _pkt) {
        m_C2SProxy.ReqUseItem(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
	public override void ReqUseItemGoods(PktInfoItemCnt _pkt) {
        m_C2SProxy.ReqUseItemGoods(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    // 곡옥 관련
    public override void ReqSellGem(PktInfoGemSell _pkt) {
        m_C2SProxy.ReqSellGem(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqSetLockGem(PktInfoGemLock _pkt) {
        m_C2SProxy.ReqSetLockGem(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqResetOptGem(PktInfoGemResetOptReq _pkt){
        m_C2SProxy.ReqResetOptGem(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqResetOptSelectGem(PktInfoGemResetOptSelect _pkt) {
        m_C2SProxy.ReqResetOptSelectGem(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqLvUpGem(PktInfoProductComGrowReq _pkt) {
        m_C2SProxy.ReqLvUpGem(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqWakeGem(PktInfoProductComGrowReq _pkt) {
        m_C2SProxy.ReqWakeGem(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
	public override void ReqEvolutionGem(PktInfoProductComGrowReq _pkt)
	{
		m_C2SProxy.ReqEvolutionGem(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
	}
	public override void ReqAnalyzeGem(PktInfoProductComGrowReq _pkt)
	{
		m_C2SProxy.ReqAnalyzeGem(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
	}

	// 무기 관련
	public override void ReqSellWeapon(PktInfoWeaponSell _pkt) {
        m_C2SProxy.ReqSellWeapon(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqSetLockWeapon(PktInfoWeaponLock _pkt) {
        m_C2SProxy.ReqSetLockWeapon(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqLvUpWeapon(PktInfoProductComGrowReq _pkt) {
        m_C2SProxy.ReqLvUpWeapon(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqWakeWeapon(PktInfoProductComGrowReq _pkt){
        m_C2SProxy.ReqWakeWeapon(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqEnchantWeapon(PktInfoProductComGrowReq _pkt){
        m_C2SProxy.ReqEnchantWeapon(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqSkillLvUpWeapon(PktInfoProductComGrowReq _pkt){
        m_C2SProxy.ReqSkillLvUpWeapon(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqApplyGemInWeapon(PktInfoWeaponSlotGem _pkt) {
        m_C2SProxy.ReqApplyGemInWeapon(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqAddSlotInWpnDepot(System.Byte _addCnt) {
        m_C2SProxy.ReqAddSlotInWpnDepot(HostID.HostID_Server, RmiContext.ReliableSend, _addCnt);
    }
    public override void ReqApplySlotInWpnDepot(PktInfoWpnDepotApply _pkt) {
        m_C2SProxy.ReqApplySlotInWpnDepot(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    // 문양 관련
    public override void ReqApplyPosBadge(PktInfoBadgeApplyPos _pkt) {
        m_C2SProxy.ReqApplyPosBadge(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqApplyOutPosBadge(PktInfoBadgeComReq _pkt) {
        m_C2SProxy.ReqApplyOutPosBadge(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqSellBadge(PktInfoBadgeSell _pkt) {
        m_C2SProxy.ReqSellBadge(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqSetLockBadge(PktInfoBadgeLock _pkt) {
        m_C2SProxy.ReqSetLockBadge(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqUpgradeBadge(PktInfoBadgeComReq _pkt) {
        m_C2SProxy.ReqUpgradeBadge(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqResetUpgradeBadge(PktInfoBadgeComReq _pkt) {
        m_C2SProxy.ReqResetUpgradeBadge(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqDecomposition(PktInfoDecompositionReq _pkt) {
        m_C2SProxy.ReqDecomposition(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
}