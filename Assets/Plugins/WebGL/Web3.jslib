mergeInto(LibraryManager.library, {
  
  InitWeb3: function(rpcPtr, keyPtr, addrPtr, abiPtr) {
    var rpc = UTF8ToString(rpcPtr);
    var key = UTF8ToString(keyPtr);
    var addr = UTF8ToString(addrPtr);
    var abi = UTF8ToString(abiPtr);
    
    window.web3 = new Web3(rpc);
    window.shop = window.web3.eth.accounts.privateKeyToAccount(key);
    window.web3.eth.accounts.wallet.add(window.shop);
    window.contract = new window.web3.eth.Contract(JSON.parse(abi), addr);
    window.contractAddr = addr;
    
    console.log('[Web3] 초기화 완료');
  },
  
  ConnectWallet: function() {
    if (!window.ethereum) {
      alert('MetaMask를 설치해주세요!');
      return;
    }
    
    window.ethereum.request({ method: 'eth_requestAccounts' })
      .then(function(accounts) {
        window.player = accounts[0];
        console.log('[Web3] 연결됨:', window.player);
        alert('지갑 연결 완료!\n주소: ' + window.player);
      })
      .catch(function(err) {
        console.error('[Web3] 연결 실패:', err);
        alert('연결 실패: ' + err.message);
      });
  },
  
  GetBalance: function() {
    if (!window.player) {
      alert('먼저 지갑을 연결해주세요!');
      return;
    }
    
    window.contract.methods.balanceOf(window.player).call()
      .then(function(balance) {
        console.log('[Web3] 잔액:', balance);
        
        // 1. alert로 표시
        alert('현재 잔액: ' + balance + ' 토큰');
        
        // 2. Unity로 전달 (TMP_Text 업데이트용)
        if (typeof unityInstance !== 'undefined') {
          unityInstance.SendMessage('BlockchainManager', 'OnBalanceReceived', balance.toString());
        }
      })
      .catch(function(err) {
        console.error('[Web3] 잔액 조회 실패:', err);
        alert('잔액 조회 실패: ' + err.message);
      });
  },
  
  BuyItem: function(amount) {
    if (!window.player) {
      alert('먼저 지갑을 연결해주세요!');
      return;
    }
    
    var data = window.contract.methods.buyItem(amount).encodeABI();
    
    window.ethereum.request({
      method: 'eth_sendTransaction',
      params: [{
        from: window.player,
        to: window.contractAddr,
        data: data,
        //gas: '0x186A0'
      }]
    })
    .then(function(hash) {
      console.log('[Web3] 구매 완료:', hash);
      alert('구매 완료!\nTx: ' + hash);
    })
    .catch(function(err) {
      console.error('[Web3] 구매 실패:', err);
      alert('구매 실패: ' + err.message);
    });
  },
  
  SellItem: function(amount) {
    if (!window.player) {
      alert('먼저 지갑을 연결해주세요!');
      return;
    }
    
    window.contract.methods.sellItem(window.player, amount)
      .send({ from: window.shop.address, gas: 100000 })
      .then(function(receipt) {
        console.log('[Web3] 판매 완료:', receipt.transactionHash);
        alert('판매 완료!\nTx: ' + receipt.transactionHash);
      })
      .catch(function(err) {
        console.error('[Web3] 판매 실패:', err);
        alert('판매 실패: ' + err.message);
      });
  },
  
  GiveReward: function(amount) {
    if (!window.player) {
      alert('먼저 지갑을 연결해주세요!');
      return;
    }
    
    window.contract.methods.giveReward(window.player, amount)
      .send({ from: window.shop.address, gas: 100000 })
      .then(function(receipt) {
        console.log('[Web3] 보상 완료:', receipt.transactionHash);
        alert('보상 지급 완료!\nTx: ' + receipt.transactionHash);
      })
      .catch(function(err) {
        console.error('[Web3] 보상 실패:', err);
        alert('보상 실패: ' + err.message);
      });
  }
  
});