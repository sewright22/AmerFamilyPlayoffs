#!/bin/bash

# Script to suppress Ubuntu login messages that confuse MCP servers
# Run this on your Ubuntu server as the deploy user

echo "🔧 Suppressing Ubuntu login messages for deploy user..."

# Create .hushlogin to suppress most login messages
touch ~/.hushlogin

# Disable update-motd for this user
sudo chmod -x /etc/update-motd.d/10-help-text 2>/dev/null || true
sudo chmod -x /etc/update-motd.d/50-motd-news 2>/dev/null || true
sudo chmod -x /etc/update-motd.d/80-esm 2>/dev/null || true
sudo chmod -x /etc/update-motd.d/95-hwe-eol 2>/dev/null || true

# Create a minimal .bashrc that doesn't show extra messages
cat > ~/.bashrc << 'EOF'
# Minimal .bashrc for deploy user to avoid confusing MCP servers

# If not running interactively, don't do anything
case $- in
    *i*) ;;
      *) return;;
esac

# Basic PATH
export PATH="/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin"

# Basic aliases
alias ll='ls -alF'
alias la='ls -A'
alias l='ls -CF'

# Don't show any extra messages
export DEBIAN_FRONTEND=noninteractive
EOF

echo "✅ Ubuntu login messages suppressed for deploy user"
echo "💡 The deploy user will now have minimal login output"