Vagrant.configure("2") do |config|
  config.vm.box = "ubuntu/bionic64"

  config.vm.provision "shell", path: "provisioning/provision.sh"

  config.vm.network "forwarded_port", guest: 5000, host: 5000

end