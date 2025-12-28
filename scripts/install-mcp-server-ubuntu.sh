#!/bin/bash

# Script to install MCP SSH Server on Ubuntu
# Run this on your Ubuntu server as the deploy user

set -e

echo "🚀 Installing MCP SSH Server on Ubuntu..."

# Check if Python 3.10+ is available
python_version=$(python3 --version 2>&1 | grep -oP '\d+\.\d+' | head -1)
if [[ $(echo "$python_version >= 3.10" | bc -l) -eq 0 ]]; then
    echo "❌ Python 3.10+ required, found $python_version"
    echo "💡 Installing Python 3.11..."
    sudo apt update
    sudo apt install -y python3.11 python3.11-venv python3.11-pip
    sudo update-alternatives --install /usr/bin/python3 python3 /usr/bin/python3.11 1
fi

# Install uv (Python package manager)
if ! command -v uv &> /dev/null; then
    echo "📦 Installing uv (Python package manager)..."
    curl -LsSf https://astral.sh/uv/install.sh | sh
    source ~/.cargo/env
    export PATH="$HOME/.cargo/bin:$PATH"
fi

# Install the MCP SSH Server
echo "🔧 Installing m2m-mcp-server-ssh-server..."
uv tool install m2m-mcp-server-ssh-server

# Create systemd service for the MCP server
echo "⚙️ Creating systemd service..."
sudo tee /etc/systemd/system/mcp-ssh-server.service > /dev/null << EOF
[Unit]
Description=MCP SSH Server
After=network.target
Wants=network.target

[Service]
Type=simple
User=deploy
Group=deploy
WorkingDirectory=/home/deploy
Environment=PATH=/home/deploy/.local/bin:/usr/local/bin:/usr/bin:/bin
ExecStart=/home/deploy/.local/bin/uv tool run m2m-mcp-server-ssh-server --port 8022
Restart=always
RestartSec=5
StandardOutput=journal
StandardError=journal

[Install]
WantedBy=multi-user.target
EOF

# Enable and start the service
echo "🔄 Starting MCP SSH Server service..."
sudo systemctl daemon-reload
sudo systemctl enable mcp-ssh-server
sudo systemctl start mcp-ssh-server

# Check service status
echo "📊 Checking service status..."
sudo systemctl status mcp-ssh-server --no-pager

# Open firewall port for MCP server
echo "🔥 Configuring firewall..."
sudo ufw allow 8022/tcp

# Test if the server is running
echo "🧪 Testing MCP server..."
sleep 5
if netstat -tlnp | grep :8022; then
    echo "✅ MCP SSH Server is running on port 8022"
else
    echo "❌ MCP SSH Server is not running"
    echo "📋 Service logs:"
    sudo journalctl -u mcp-ssh-server --no-pager -n 20
fi

echo ""
echo "🎉 MCP SSH Server installation completed!"
echo ""
echo "📋 Service Management Commands:"
echo "  Check status: sudo systemctl status mcp-ssh-server"
echo "  View logs:    sudo journalctl -u mcp-ssh-server -f"
echo "  Restart:      sudo systemctl restart mcp-ssh-server"
echo "  Stop:         sudo systemctl stop mcp-ssh-server"
echo ""
echo "🌐 The MCP server is now running on port 8022"
echo "🔧 Update your MCP client configuration to connect to port 8022"