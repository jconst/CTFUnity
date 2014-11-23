using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Boost : DropItem
{
    const float shadowFadeoutTime = 0.3f;
    const float secondsBetweenShadowDrops = 0.05f;
    const float shadowStartAlpha = 0.7f;
    float timeUntilShadowDrop = 0f;

    public override bool TryDrop(Player p) {
        base.TryDrop(p);

        if (p.currentBoost > 1f) {
            return false;
        }

        manaCost = 3;
        lifeTime = 4f;
        lifeTime += 1f; //to account for fadeout
        owner.currentBoost = 2f;
        return true;
    }

    public new void Update() {
        base.Update();

        if (lifeTime < 1) {
            Deactivate();
            return;
        }

        transform.position = owner.transform.position;

        timeUntilShadowDrop -= Time.deltaTime;
        if (timeUntilShadowDrop < 0) {
            StartCoroutine(DropShadow());
            timeUntilShadowDrop = secondsBetweenShadowDrops;
        }
    }

    IEnumerator DropShadow() {
        GameObject shadow = Instantiate(owner.gameObject) as GameObject;
        shadow.GetComponents<Behaviour>()
              .ToList()
              .ForEach(c => c.enabled = false);
        shadow.collider2D.enabled = false;
        shadow.rigidbody2D.isKinematic = true;

        const int fadeOutSmoothness = 10;
        for (int i=fadeOutSmoothness; i>=0; i--) {
            Color fadedColor = shadow.renderer.material.color;
            fadedColor.a = shadowStartAlpha * ((float)i / (float)fadeOutSmoothness);
            shadow.renderer.material.color = fadedColor;
            yield return new WaitForSeconds(shadowFadeoutTime / fadeOutSmoothness);
        }
        Destroy(shadow);
    }

    void Deactivate() {
        owner.currentBoost = 1f;
    }
}
