package com.nsdey.minesweeper

import android.content.Context
import android.view.View
import java.util.Collections
import java.util.WeakHashMap

/** Shared multiplier for every Minesweeper pixel, sprite, and panel measurement. */
object GameScale {
    const val DEFAULT_SCALE = 1.0f

    /** Current project setting. Change to 2.0f for twice the drawable size. */
    var scale: Float = 3.0f
        set(value) {
            require(value.isFinite() && value > 0f) { "Game scale must be a positive finite number." }
            if (field != value) {
                field = value
                observers.toList().forEach { view ->
                    view.requestLayout()
                    view.invalidate()
                }
            }
        }

    private val observers = Collections.newSetFromMap(WeakHashMap<View, Boolean>())

    fun scaledPixels(context: Context, sourcePixels: Float): Float =
        sourcePixels * scale * context.resources.displayMetrics.density

    internal fun observe(view: View) {
        observers.add(view)
    }

    internal fun stopObserving(view: View) {
        observers.remove(view)
    }
}
