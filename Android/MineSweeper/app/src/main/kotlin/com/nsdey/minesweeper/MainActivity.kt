package com.nsdey.minesweeper

import android.app.Activity
import android.os.Build
import android.os.Bundle
import android.view.Gravity
import android.view.View
import android.view.WindowInsets
import android.view.WindowInsetsController
import android.widget.Button
import android.widget.LinearLayout
import android.widget.PopupMenu
import android.widget.TextView

class MainActivity : Activity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enterImmersiveFullscreen()
        setContentView(createMainScreen())
    }

    private fun enterImmersiveFullscreen() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) {
            window.setDecorFitsSystemWindows(false)
            window.insetsController?.let { controller ->
                controller.hide(WindowInsets.Type.statusBars() or WindowInsets.Type.navigationBars())
                controller.systemBarsBehavior =
                    WindowInsetsController.BEHAVIOR_SHOW_TRANSIENT_BARS_BY_SWIPE
            }
        } else {
            @Suppress("DEPRECATION")
            window.decorView.systemUiVisibility = (
                View.SYSTEM_UI_FLAG_FULLSCREEN
                    or View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
                    or View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY
                    or View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                )
        }
    }

    private fun createMainScreen(): View {
        val page = LinearLayout(this).apply {
            orientation = LinearLayout.VERTICAL
            setBackgroundColor(0xFFF7F7F7.toInt())
        }

        val topBar = LinearLayout(this).apply {
            orientation = LinearLayout.HORIZONTAL
            gravity = Gravity.CENTER_VERTICAL
            setPadding(dp(16), 0, dp(8), 0)
            setBackgroundColor(0xFFFFFFFF.toInt())
        }

        val title = TextView(this).apply {
            text = getString(com.nsdey.minesweeper.R.string.app_name)
            textSize = 20f
            setTextColor(0xFF202124.toInt())
        }

        val menuButton = Button(this).apply {
            text = "⋮"
            textSize = 24f
            contentDescription = "More options"
            setOnClickListener { anchor ->
                PopupMenu(this@MainActivity, anchor).apply {
                    menu.add("New game")
                    menu.add("Settings")
                    show()
                }
            }
        }

        topBar.addView(
            title,
            LinearLayout.LayoutParams(0, dp(56), 1f)
        )
        topBar.addView(menuButton, LinearLayout.LayoutParams(dp(48), dp(56)))
        page.addView(topBar)

        val placeholder = TextView(this).apply {
            text = "Game screen design goes here"
            textSize = 16f
            setTextColor(0xFF5F6368.toInt())
            gravity = Gravity.CENTER
        }
        page.addView(
            placeholder,
            LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MATCH_PARENT,
                0,
                1f
            )
        )
        return page
    }

    private fun dp(value: Int): Int =
        (value * resources.displayMetrics.density).toInt()
}
