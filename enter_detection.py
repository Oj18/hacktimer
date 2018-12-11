from pynput import keyboard
import time
import _thread
import sys

def on_press(key):
    if (key == keyboard.Key.enter):
        sys.stdout.write("next")
        sys.stdout.flush()

        raise SystemExit

    if (key == keyboard.Key.esc):
        sys.stdout.write("stop")
        sys.stdout.flush()

        raise SystemExit


def startListener():
    # Collect events until released
    with keyboard.Listener(
        on_press=on_press) as listener:
        listener.join()

startListener()